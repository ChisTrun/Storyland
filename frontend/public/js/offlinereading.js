let rendition;
let book;

$(document).ready(function () {
  $('#file').on('change', uploadFile);
});

async function uploadFile() {
  const fileInput = $('#file')[0];
  const file = fileInput.files[0];
  const errorDiv = $('#error');

  errorDiv.css('display', 'none');
  errorDiv.text('');

  if (!file) {
    errorDiv.text('Vui lòng tải lên tệp hợp lệ.');
    errorDiv.css('display', 'block');
    return;
  }

  const fileExtension = file.name.split('.').pop().toLowerCase();

  clearPreviousFile();

  if (fileExtension === 'pdf') {
    const pdfURL = URL.createObjectURL(file);

    const pdfViewer = $('#pdfViewer');
    const pdfObject = $('#pdfObject');
    const pdfLink = $('#pdfLink');

    pdfObject.attr('data', pdfURL);
    pdfLink.attr('href', pdfURL);

    $('#epubViewer').css('display', 'none');
    pdfViewer.css('display', 'block');
  } else if (fileExtension === 'epub') {
    try {
      const epubBlob = await readFileAsBlob(file);
      await saveToLocal(epubBlob, file.name);

      const epubViewer = $('#epubViewer');
      const viewerElement = $('#viewer')[0];

      book = ePub(epubBlob);
      rendition = book.renderTo(viewerElement.contentDocument.body, {
        width: "auto",
        height: "100%", // Ensure the iframe height is set to 100%
      });

      rendition.display().catch(error => {
        errorDiv.text('Không thể tải EPUB: ' + error.message);
        errorDiv.css('display', 'block');
        console.error('Lỗi hiển thị EPUB:', error);
      });

      $('#pdfViewer').css('display', 'none');
      epubViewer.css('display', 'block');

      await loadTOC();
    } catch (error) {
      errorDiv.text(error.message);
      errorDiv.css('display', 'block');
    }
  } else {
    errorDiv.text('Định dạng tệp không được hỗ trợ. Vui lòng tải lên tệp PDF hoặc EPUB.');
    errorDiv.css('display', 'block');
  }
}

function clearPreviousFile() {
  // Clear PDF Viewer
  $('#pdfObject').attr('data', '');

  // Clear EPUB Viewer and TOC
  $('#toc').empty();
  $('#viewer').attr('src', '');

  if (rendition) {
    rendition.destroy();
    rendition = null;
  }

  $('#epubViewer').css('display', 'none');
  $('#pdfViewer').css('display', 'none');
}

async function readFileAsBlob(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = function (event) {
      const blob = new Blob([event.target.result], { type: file.type });
      resolve(blob);
    };
    reader.onerror = function (error) {
      reject(error);
    };
    reader.readAsArrayBuffer(file);
  });
}

function openDatabase() {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open('epubLibrary', 1);

    request.onupgradeneeded = function (event) {
      const db = event.target.result;
      if (!db.objectStoreNames.contains('epubs')) {
        db.createObjectStore('epubs', { keyPath: 'name' });
      }
    };

    request.onsuccess = function (event) {
      resolve(event.target.result);
    };

    request.onerror = function (event) {
      reject(event.target.error);
    };
  });
}

async function saveToLocal(blob, fileName) {
  const db = await openDatabase();
  const transaction = db.transaction(['epubs'], 'readwrite');
  const store = transaction.objectStore('epubs');

  return new Promise((resolve, reject) => {
    const request = store.put({ name: fileName, blob });

    request.onsuccess = function () {
      resolve();
    };

    request.onerror = function (event) {
      reject(event.target.error);
    };
  });
}

async function loadFromLocal(fileName) {
  const db = await openDatabase();
  const transaction = db.transaction(['epubs'], 'readonly');
  const store = transaction.objectStore('epubs');

  return new Promise((resolve, reject) => {
    const request = store.get(fileName);

    request.onsuccess = function (event) {
      const result = event.target.result;
      if (result) {
        const blob = result.blob;
        resolve(URL.createObjectURL(blob));
      } else {
        reject('Không tìm thấy tệp EPUB trong kho lưu trữ cục bộ.');
      }
    };

    request.onerror = function (event) {
      reject(event.target.error);
    };
  });
}

async function loadTOC() {
  const toc = await book.loaded.navigation;
  const tocElement = $('#toc');
  tocElement.empty();

  function traverseTOC(items) {
    items.forEach(item => {
      const tocItem = $('<div class="toc-item"></div>');
      tocItem.text(item.label);
      tocItem.on('click', () => {
        rendition.display(item.href).catch(error => {
          console.error('Lỗi chuyển đến chương:', error);
        });
      });
      tocElement.append(tocItem);
      if (item.subitems) {
        const subItemContainer = $('<div class="sub-items"></div>');
        tocItem.append(subItemContainer);
        traverseTOC(item.subitems);
      }
    });
  }

  traverseTOC(toc);
}