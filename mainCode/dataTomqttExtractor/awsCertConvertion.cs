using MQTTnet;
using MQTTnet.Client.Options;
using Oocx.ReadX509CertificateFromPem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MQTTNet_AWS
{
    class awsCertConvertion
    {


        public awsCertConvertion(string name)
        {
            this.name = name;
            rootCertificateTrust = new RootCertificateTrust();

            deviceCertPEMString = "";
            devicePrivateCertPEMString = "";
            certificateAuthorityCertPEMString = "";

            certAWS = new List<X509Certificate>();

        }

        private string name;

        private RootCertificateTrust rootCertificateTrust;
        private string certificateAuthorityCertPEMString;
        private string deviceCertPEMString;
        private string devicePrivateCertPEMString;

        public List<X509Certificate> certAWS;

        //static async Task Main(string[] args)
        public List<X509Certificate>? certTask()
        {
            try
            {
                deviceCertPEMString = File.ReadAllText(@".\certs\agent-certificate.pem.crt");
                devicePrivateCertPEMString = File.ReadAllText(@".\certs\agent-private.pem.key");
                certificateAuthorityCertPEMString = File.ReadAllText(@".\certs\AmazonRootCA1.pem");
            }
            catch
            {
                return null;
            }


            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();


            //Converting from PEM to X509 certs in C# is hard
            //Load the CA certificate
            //https://gist.github.com/ChrisTowles/f8a5358a29aebcc23316605dd869e839
            var certBytes = Encoding.UTF8.GetBytes(certificateAuthorityCertPEMString);
            var signingcert = new X509Certificate2(certBytes);

            //Load the device certificate
            //Use Oocx.ReadX509CertificateFromPem to load cert from pem
            var reader = new CertificateFromPemReader();
            X509Certificate2 deviceCertificate = reader.LoadCertificateWithPrivateKeyFromStrings(deviceCertPEMString, devicePrivateCertPEMString);



            //This is a helper class to allow verifying a root CA separately from the Windows root store
            rootCertificateTrust = new RootCertificateTrust();
            rootCertificateTrust.AddCert(signingcert);


            // Certificate based authentication
            List<X509Certificate> certs = new List<X509Certificate>
            {
                signingcert,
                deviceCertificate
            };

            certAWS = certs;

            return certs;

        }
    }

    /// <summary>
    /// Verifies certificates against a list of manually trusted certs.
    /// If a certificate is not in the Windows cert store, this will check that it's valid per our internal code.
    /// </summary>
    internal class RootCertificateTrust
    {

        X509Certificate2Collection certificates;
        internal RootCertificateTrust()
        {
            certificates = new X509Certificate2Collection();
        }

        /// <summary>
        /// Add a trusted certificate
        /// </summary>
        /// <param name="x509Certificate2"></param>
        internal void AddCert(X509Certificate2 x509Certificate2)
        {
            certificates.Add(x509Certificate2);
        }

        /// <summary>
        /// This matches the delegate signature expected for certificate verification for MQTTNet
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>

        internal bool VerifyServerCertificate(MqttClientCertificateValidationCallbackContext arg) => VerifyServerCertificate(new object(), arg.Certificate, arg.Chain, arg.SslPolicyErrors);

        //internal bool VerifyServerCertificate(MqttClientCertificateValidationEventArgs arg) => VerifyServerCertificate(new object(), arg.Certificate, arg.Chain, arg.SslPolicyErrors);

        /// <summary>
        /// This matches the delegate signature expected for certificate verification for M2MQTT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        internal bool VerifyServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            if (sslPolicyErrors == SslPolicyErrors.None) return true;

            X509Chain chainNew = new X509Chain();
            var chainTest = chain;

            chainTest.ChainPolicy.ExtraStore.AddRange(certificates);

            // Check all properties
            chainTest.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

            // This setup does not have revocation information
            chainTest.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            // Build the chain
            var buildResult = chainTest.Build(new X509Certificate2(certificate));

            //Just in case it built with trust
            if (buildResult) return true;

            //If the error is something other than UntrustedRoot, fail
            foreach (var status in chainTest.ChainStatus)
            {
                if (status.Status != X509ChainStatusFlags.UntrustedRoot)
                {
                    return false;
                }
            }

            //If the UntrustedRoot is on something OTHER than the GreenGrass CA, fail
            foreach (var chainElement in chainTest.ChainElements)
            {
                foreach (var chainStatus in chainElement.ChainElementStatus)
                {
                    if (chainStatus.Status == X509ChainStatusFlags.UntrustedRoot)
                    {
                        var found = certificates.Find(X509FindType.FindByThumbprint, chainElement.Certificate.Thumbprint, false);
                        if (found.Count == 0) return false;
                    }
                }
            }

            return true;
        }

    }

}