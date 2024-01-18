# Use an official Node.js runtime as a base image
FROM node:14

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy package.json and package-lock.json to the container
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code to the container
COPY . .

# Expose the port your app runs on (assuming it's 3000, update if needed)
EXPOSE 3000
EXPOSE 27017

# Command to start your application
CMD ["node", "app.js"]