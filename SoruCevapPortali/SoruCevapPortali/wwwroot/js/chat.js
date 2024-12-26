"use strict";

// SignalR bağlantısını başlat
var connection = new signalR.HubConnectionBuilder().withUrl("/todoHub").build();

// Gönderim butonunu, bağlantı kurulana kadar devre dışı bırak
document.getElementById("sendButton").disabled = true;

// Mesaj aldıkça ekrana yeni bir görev ekle
connection.on("ReceiveMessage", function (name) {
    // Yeni bir liste öğesi (li) oluştur
    var li = document.createElement("li");
    li.className = "list-group-item d-flex align-items-center justify-content-between"; // Listeye stil ekle

    // İçeriği oluştur
    li.innerHTML = `
        <span>
            ${name}
        </span>
        <div>
            <a href="#" class="btn btn-danger delete-btn" data-name="${name}">Delete</a>
        </div>
    `;

    // Listeye (messageList) öğeyi ekle
    var messageList = document.getElementById("messagesList");
    if (messageList) {
        messageList.appendChild(li);
    }

    // Silme butonuna tıklandığında işlemi başlat
    var deleteButton = li.querySelector(".delete-btn"); // Silme butonunu seç
    if (deleteButton) {
        deleteButton.addEventListener("click", function () {
            // Görevi sil
            li.remove();

            // SignalR üzerinden silme işlemini başlat
            var taskName = deleteButton.getAttribute("data-name"); // Name değerini al
            connection.invoke("DeleteTask", taskName) // Burada name parametresini kullanıyoruz
                .catch(function (err) {
                    return console.error(err.toString());
                });
        });
    }
});

// Bağlantı başarıyla başlatıldığında, gönderim butonunu aktif et
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

// Gönderim butonuna tıklandığında mesajı SignalR'e gönder
document.getElementById("sendButton").addEventListener("click", function (event) {
    var name = document.getElementById("Name").value; // İsim (name) al

    // Gönderiyi SignalR'e gönder
    connection.invoke("SendMessage", name)
        .catch(function (err) {
            return console.error(err.toString());
        });
    event.preventDefault(); // Sayfa yenilemesini engelle
});

// SignalR üzerinden gelen silme bildirimine göre öğeyi sil
connection.on("TaskDeleted", function (name) {
    // Silinen görevin name'ini al ve sayfada ilgili öğeyi sil
    var messageList = document.getElementById("messagesList");
    if (messageList) {
        var items = messageList.getElementsByTagName("li");
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var deleteButton = item.querySelector(".delete-btn");
            if (deleteButton && deleteButton.getAttribute("data-name") === name) {
                messageList.removeChild(item); // İlgili öğeyi sil
                break;
            }
        }
    }
});
