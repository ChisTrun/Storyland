class ErrorDisplay {
    constructor(title, code, message) {
        this.title = title;
        this.code = code;
        this.message = message;
    }
}

function logDisplay(err, req, res, next) {
    console.error(err);
    console.error(err.stack);
    return next(err);
}

function xmlhttpError(err, req, res, next) {
    if (req.xhr) {
        return res.status(500).send({ error: 'Something failed!' });
    } else {
        return next(err);
    }
}

function predictedErrorPageDisplay(err, req, res, next) {
    if (err instanceof ErrorDisplay) {
        return res.status(err.code).render('error/error', {
            layout: 'plain',
            name: err.title,
            message: err.message,
        }, null);
    } else {
        return next(err);
    }
}

function finalHandler(err, req, res, next) {
    return res.status(500).send('Something went down ... (:<)');
}

module.exports = { ErrorDisplay, logDisplay, xmlhttpError, predictedErrorPageDisplay, finalHandler };