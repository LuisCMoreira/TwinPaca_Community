const express = require('express');
const mongoose = require('mongoose');
const http = require('http');
const socketIo = require('socket.io');
const app = express();
const server = http.createServer(app);
const io = socketIo(server);
const port = 3000;

// Serve static files from a directory (e.g., public)
app.use(express.static('public'));

// Define a route for your web page
app.get('/webpage', (req, res) => {
  res.sendFile(__dirname + '/public/webpage.html');
});

// 1. Connect to MongoDB
mongoose.connect('mongodb://localhost:27017/mqtt', { useNewUrlParser: true, useUnifiedTopology: true });

const db = mongoose.connection;

db.on('error', console.error.bind(console, 'MongoDB connection error:'));
db.once('open', () => {
  console.log('Connected to MongoDB');
});

// 2. Define the MongoDB Model for the "brokertopics" Collection
const mqttbroker = require('./brokerModel'); // Adjust the path as needed

// Create a WebSocket connection
io.on('connection', (socket) => {
  console.log('A user connected');

  // Set up a recurring interval to fetch and send data to connected clients every second
  const fetchAndSendData = async () => {
    try {
      const lastEvent = await mqttbroker.findOne().sort({ timestamp: -1 }).exec();

      if (lastEvent) {
        socket.emit('mqtt-data', lastEvent);
      }
    } catch (err) {
      console.error(err);
    }
  };

  // Set up the initial data fetch
  fetchAndSendData();

  // Set up the recurring data updates
  const updateInterval = setInterval(fetchAndSendData, 1000);

  // When the client disconnects, clear the update interval
  socket.on('disconnect', () => {
    console.log('User disconnected');
    clearInterval(updateInterval);
  });
});

server.listen(port, () => {
  console.log(`Server is listening on port ${port}`);
});
