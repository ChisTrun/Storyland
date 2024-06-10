const { express, https, fs } = require('./global/lib');
const { HOST, PORT } = require('./global/env');
const cors = require('cors');
const app = express();

app.use(cors());
require('./config/session.config')(app);
require('./config/reader.config')(app);
require('./config/hbs.config')(app);
require('./config/router.config')(app);


const server = https.createServer({
    key: fs.readFileSync('./_certs/secretkey.key'),
    cert: fs.readFileSync('./_certs/secretcert.cert')
}, app);

server.listen(PORT, HOST, () => {
    console.log(`App Server is on: https://${HOST}:${PORT}`);
});