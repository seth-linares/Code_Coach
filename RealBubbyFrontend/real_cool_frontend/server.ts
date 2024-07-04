// import { createServer } from 'https'; // Import the createServer function from the https module to create an HTTPS server
// import { parse } from 'url'; // Import the parse function from the url module to parse URL strings
// import next from 'next'; // Import the next function from the next module to initialize a Next.js app
// import fs from 'fs'; // Import the fs (File System) module to read files
// import path from 'path'; // Import the path module to work with file and directory paths
// import { NextServer, RequestHandler } from 'next/dist/server/next'; // Import specific types from Next.js for better TypeScript support
//
// // Determine if we're running in development or production mode
// // The NODE_ENV environment variable is commonly used to differentiate between development and production environments
// const dev: boolean = process.env.NODE_ENV !== 'production';
//
// // Initialize the Next.js app with the dev variable to specify the mode (development or production)
// const app: NextServer = next({ dev });
//
// // Get the request handler from the Next.js app
// // The request handler handles all incoming requests to the Next.js app
// const handle: RequestHandler = app.getRequestHandler();
//
// // Define HTTPS options with SSL certificate files
// // These certificates are needed to enable HTTPS
// const httpsOptions: { key: Buffer, cert: Buffer } = {
//     // Read the private key file
//     key: fs.readFileSync(path.join(__dirname, 'certs', 'localhost+2-key.pem')),
//     // Read the certificate file
//     cert: fs.readFileSync(path.join(__dirname, 'certs', 'localhost+2.pem'))
// };
//
// // Define the port to run the server on
// // You can use an environment variable for flexibility, defaulting to 3000 if not set
// const port: string | number = process.env.PORT || 3000;
//
// // Prepare the Next.js app
// app.prepare().then((): void => {
//     // Create an HTTPS server with the specified options and request handler
//     createServer(httpsOptions, (req, res) => {
//         // Parse the incoming request URL
//         const parsedUrl = parse(req.url!, true);
//         // Handle the request using the Next.js request handler
//         handle(req, res, parsedUrl)
//             .catch(err => {
//                 // Log any errors that occur while handling the request
//                 console.error('Error occurred handling', req.url, err);
//                 // Respond with a 500 Internal Server Error status code
//                 res.statusCode = 500;
//                 res.end('Internal Server Error');
//             });
//         // Listen on the specified port for incoming connections
//     }).listen(port, (err?: Error): void => {
//         if (err) throw err; // If there's an error starting the server, throw it
//         console.log(`> Ready on https://localhost:${port}`); // Log a message indicating the server is ready
//     });
// });
