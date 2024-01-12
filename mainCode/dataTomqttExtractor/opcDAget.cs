using GodSharp.Opc.Da;
using GodSharp.Opc.Da.Options;

namespace DAgent
{
    class opcDAget
    {
        public opcDAget(string name)
            {
                this.name = name;
                outget = new List<Object[]>();

                
            } 
        
public IOpcDaClient? DAclient(string IP,string serverName,List<string> TagsToGet)
{
        
        var groups = new List<GroupData>
            {
            new GroupData
                {
                Name = "default", UpdateRate = 100, ClientHandle = 010, IsSubscribed = true,
                Tags = new List<Tag>{}
                }
            };



            foreach (var ToGet in TagsToGet)
            {
                var tag = new Tag(ToGet, 011);
                groups[0].Tags.Add(tag);
            }

            if ( IP == null || serverName == null)
            {
                Console.WriteLine("Missing opcDA config file!!!");
                return null;
            }
            var server = new ServerData
            {
                 
                Host = IP,
                ProgId = serverName,
                // initial with data info,after connect will be add to client auto
                // if this is null,you should add group and tag manually
                Groups = groups
            };

            var client = DaClientFactory.Instance.CreateOpcNetApiClient(new DaClientOptions(
                server,
                OnDataChangedHandler,
                OnShoutdownHandler,
                OnAsyncReadCompletedHandler,
                OnAsyncWriteCompletedHandler));


            return client;


}

public List<Object[]> opcdaGet(IOpcDaClient? client)
{

                if (client != null)
                {
                outget.Clear();

                if (!client.Connected)
                {client.Connect();}
                

                foreach (var group in client.Groups.Values)
            {
                if (group.Tags.Count == 0) continue;
                var results = group.Reads(group.Tags.Values.Select(x => x.ItemName)?.ToArray());

                
                foreach (var item in results)
                {

                        if (item.Result.Value != null)
                        {
                            var aget= new String[]
                            {
                                item.Result.ItemName.ToString(),
                                null+item.Result.Value,
                                null+item.Result.Timestamp
                                
                            };

                            outget.Add(aget);
                        };
                }
            }

            //client.Disconnect();
            //client.Dispose();

            }
            return outget;

}

        public static void OnDataChangedHandler(DataChangedOutput output)
        {
            //Console.WriteLine($"{output.Data.ItemName}:{output.Data.Value},{output.Data.Quality} / {output.Data.Timestamp}");
        }

        public static void OnAsyncReadCompletedHandler(AsyncReadCompletedOutput output)
        {
            //Console.WriteLine(
            //    $"Async Read {output.Data.Result.ItemName}:{output.Data.Result.Value},{output.Data.Result.Quality} / {output.Data.Result.Timestamp} / {output.Data.Code}");
        }

        public static void OnAsyncWriteCompletedHandler(AsyncWriteCompletedOutput output)
        {
            //Console.WriteLine($"Async Write {output.Data.Result.ItemName}:{output.Data.Code}");
        }

        public static void OnShoutdownHandler(Server server, string reason)
        {
        }

        public string name;

        public List<Object[]> outget;
        
    }
}