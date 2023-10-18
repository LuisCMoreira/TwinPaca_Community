import paho.mqtt.client as mqtt
import json
from pymongo import MongoClient
from datetime import datetime

# MQTT Broker Config
mqtt_broker_host = "localhost"
mqtt_broker_port = 1883
mqtt_username = ""
mqtt_password = ""

# MongoDB Config
mongo_client = MongoClient("mongodb://localhost:27017/")
db = mongo_client["mqtt"]
collection = db["brokertopics"]

# MQTT on_connect callback
def on_connect(client, userdata, flags, rc):
    print(f"Connected to MQTT Broker with result code {rc}")
    # Subscribe to all topics
    client.subscribe("#")

# MQTT on_message callback
def on_message(client, userdata, msg):
    topic = msg.topic
    payload = msg.payload.decode("utf-8")

    # Check if the payload is empty
    if not payload:
        print(f"Received empty message on topic {topic}")
        return

    try:
        data = json.loads(payload)
        
        # Retrieve the existing document from MongoDB or create an empty one
        existing_document = collection.find_one({})

        if existing_document is None:
            existing_document = {}

        # Update the existing JSON structure with the new data and updatedAt timestamp
        update_json_structure(topic, data, existing_document)

        # Update the updatedAt timestamp
        update_timestamp(topic, existing_document)

        # Update the MongoDB document with the updated JSON structure
        collection.update_one({"_id": "topic_values"}, {"$set": existing_document}, upsert=True)

        print(f"Updated data in MongoDB for topic {topic}: {data}")
    except json.JSONDecodeError as e:
        print(f"Failed to parse JSON from topic {topic}: {e}")
    except Exception as e:
        print(f"Error: {e}")

def update_json_structure(topic, data, json_structure):
    topic_parts = topic.split("/")
    current = json_structure
    for part in topic_parts:
        if part not in current:
            current[part] = {}
        current = current[part]
        if part == topic_parts[-1]:
            current["value"] = data

def update_timestamp(topic, json_structure):
    current = json_structure
    for part in topic.split("/"):
        current = current[part]
    current["updated_at"] = datetime.now()

# Create MQTT client
client = mqtt.Client()
client.username_pw_set(mqtt_username, mqtt_password)
client.on_connect = on_connect
client.on_message = on_message

# Connect to MQTT broker
client.connect(mqtt_broker_host, mqtt_broker_port, keepalive=60)

# Start MQTT client loop
client.loop_forever()
