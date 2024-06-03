const { BE_HOST } = require('../global/env');

async function getServerArr() {
    const response = await fetch(`${BE_HOST}/api/server`);
    if (!response.ok) {
        const errorMessage = await serverResponse.text();
        throw Error(errorMessage);
    }         
    const resBody = await response.json();
    return resBody;
}

module.exports = { getServerArr }