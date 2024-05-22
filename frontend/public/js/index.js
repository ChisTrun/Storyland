function directTo(url) {
    location.href = url;
}

const GetServer = async () => {
    if (!$('#staticBackdrop').is(':visible')) {
        rsp = await fetch(`${host}/extension/server`)
        data = await rsp.json()
        let modal = $('.server-modal')
        modal.empty()
        data.forEach((server,index) => {
            modal.append(`
            <div class="form-check server-modal">
                <input class="form-check-input" ${index == serverIndex  ? "checked" : ""} type="radio" name="flexRadioDefault" value="${server.index}" id="flexRadioDefault1">
                <label class="form-check-label" for="flexRadioDefault1">
                    ${server.name}
                </label>
            </div>
            `)
        });
    }
   
}

setInterval(GetServer, 100);


let saveButton = $('#save-btn')
saveButton.click(async () => {
    fetch(`${host}/extension/server/set`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: 'same-origin',
        body: JSON.stringify({
            index:  parseInt($('input[name="flexRadioDefault"]:checked').val()),
        })
    })
    location.reload()
})