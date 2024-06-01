const view = 'error';
const render = {
    layout: 'plain',
    title: 'Error Page | StoryLand',
};

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
        render.name = err.title;
        render.code = err.code;
        render.message = err.message;
        return res.status(err.code).render(view, render, null);
    } else {
        return next(err);
    }
}

function finalHandler(err, req, res, next) {
    render.name = 'Something went down ... (:<)';
    render.code = 500;
    render.message = err.message;
    return res.status(500).render(view, render, null);
}

module.exports = { ErrorDisplay, logDisplay, xmlhttpError, predictedErrorPageDisplay, finalHandler };