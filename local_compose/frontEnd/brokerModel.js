const mongoose = require('mongoose');

const brokerSchema = new mongoose.Schema({
  data: mongoose.Schema.Types.Mixed, // Use Mixed data type for variable schema
});

module.exports = mongoose.model('brokertopics', brokerSchema);

