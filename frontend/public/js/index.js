const directTo = (url) => {
    location.href = url;
};

const handleError = async (errorMsg) => {
    try {
        await fetch('/error-handler', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                error: errorMsg
            })
        });

        window.location.href = '/home';
    } catch (error) {
        console.error('Failed to report error to server:', error);
    }
}

const areEqualArr = (array1, array2) => {
    if (array1.length !== array2.length) {
        return false;
    }
    const sortedArray1 = array1.slice().sort();
    const sortedArray2 = array2.slice().sort();
    for (let i = 0; i < sortedArray1.length; i++) {
        if (sortedArray1[i] !== sortedArray2[i]) {
            return false;
        }
    }
    return true;
};

const getNewServerIds = (oldServerIds, newServerIds) => {
    const sortedOldIdsInNewIds = oldServerIds.filter(id => newServerIds.includes(id));
    const newIdsNotInOldIds = newServerIds.filter(id => !oldServerIds.includes(id));
    return [...sortedOldIdsInNewIds, ...newIdsNotInOldIds];
};

const updateOrder = () => {
    $('#sortable .ui-state-default').each(function (index) {
        $(this).find('.source-index').html(`<span class='text-muted'>#${index + 1}&nbsp&nbsp</span>`);
    });
};

const getCategory = async () => {
    const dropdownList = $('.dropdown-list');
    $.ajax({
        url: '/category/all',
        method: 'GET',
        success: function (data) {
            dropdownList.empty();
            data.map(e => {
                dropdownList.append(`
                <a href="/category/${e.name}?id=${encodeURIComponent(e.id)}" class="col-2 my-2" type="button">${e.name}</a>
                `);
            });
        },
        error: function (xhr, status, error) {
            dropdownList.empty();
            console.log('Error getting categories: ', error);
        }
    });
};

const getServer = async () => {
    if (!$('#staticBackdrop').is(':visible')) {
        const response = await fetch(`${host}/extension/server`);
        const resBody = await response.json();
        const newServerIds = resBody.map(server => server.id);
        if (!areEqualArr(newServerIds, sortedServerIds)) {
            getCategory();
            sortedServerIds = getNewServerIds(sortedServerIds, newServerIds);
            await fetch(`${host}/extension/server/set`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    sortedServerIds: sortedServerIds,
                })
            });
            if (unchangedServerId != undefined && unchangedServerId != null && !sortedServerIds.includes(unchangedServerId)) {
                handleError('Server không còn khả dụng!');
            }
        }
        const modal = $('.server-modal');
        modal.empty();
        sortedServerIds.map(serverId => {
            const serverName = resBody.find(server => server.id === serverId).name;
            modal.append(`
            <div class="ui-state-default d-flex" data-id=${serverId}>
                <span class="source-index"></span>
                <span>${serverName}</span>
                <span class="flex-grow-1"></span>
                <span class="grab-label">
                    <i class="fa-solid fa-grip-lines pe-2"></i>
                    <span></span>
                </span>
            </div>
            `);
        });
        updateOrder();
    }
};

$('#sort-close-btn').on('click', () => {
    getServer();
});

$('#sort-cancel-btn').on('click', () => {
    getServer();
});

$(() => {
    $("#sortable").sortable({
        placeholder: "ui-state-highlight",
        update: function (event, ui) {
            updateOrder();
        }
    });
    $("#sortable").disableSelection();
});

$('#save-btn').click(async () => {
    const newSortedServerIds = [];
    $('#sortable .ui-state-default').each(function () {
        const serverId = $(this).data('id');
        newSortedServerIds.push(serverId);
    });
    await fetch(`${host}/extension/server/set`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: 'same-origin',
        body: JSON.stringify({
            sortedServerIds: newSortedServerIds,
        })
    });
    const url = new URL(window.location.href);
    const params = new URLSearchParams(url.search);
    if (params.has('page')) {
        params.delete('page');
        url.search = params.toString();
    }
    location.href = url.toString();
});

const toggleCtgDropdown = () => {
    $('.category-dropdown').children('i').toggleClass('fa-rotate-270');
    $('.dropdown-content').toggleClass('d-none');
}

$('.category-dropdown').on('click', function () {
    toggleCtgDropdown();
});

$('.web-mode').on('click', async function () {
    const doc = $(document.documentElement);
    const theme = doc.attr('data-theme');
    const iconMode = $(this);
    $.ajax({
        url: '/change-dark-mode',
        method: 'POST',
        data: {
            'curMode': theme,
        },
        success: function (data) {
            iconMode.toggleClass('fa-sun');
            iconMode.toggleClass('fa-moon');
            const isDark = data.isDark;
            doc.attr('data-theme', isDark == true ? 'dark' : 'light');
        },
        error: function (xhr, status, error) {
            console.error("Error changing dark mode: ", error);
        }
    });
});

$(document).on("scroll", () => {
    $(window).scrollTop() > 100 ? $("#up-btn").fadeIn() : $("#up-btn").fadeOut();
});

$("#up-btn").on("click", () => {
    return $("body,html").animate({
        scrollTop: 0
    }, 800), !1;
});

$('body').on('click', function (event) {
    if ($('.dropdown-content').is(':visible') && !$(event.target).closest('.dropdown-content').length && !$(event.target).closest('.category-dropdown').length) {
        toggleCtgDropdown();
    }
})

$(document).ready(function () {
    const errorToast = $('#error-toast');
    if (errorToast.length) {
        const toast = new bootstrap.Toast(errorToast[0]);
        toast.show();
    }
});

getServer();
setInterval(getServer, 3000);
getCategory();