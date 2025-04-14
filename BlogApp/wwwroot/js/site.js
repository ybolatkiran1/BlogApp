document.addEventListener("DOMContentLoaded", function() {
    // Aktif sayfa için navbar linklerini vurgula
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    
    navLinks.forEach(link => {
        const linkPath = link.getAttribute('href').toLowerCase();
        const linkController = link.getAttribute('asp-controller')?.toLowerCase();
        const linkAction = link.getAttribute('asp-action')?.toLowerCase();
        
        // URL kontrolü
        if (currentPath === linkPath) {
            link.classList.add('active');
        }
        // Controller ve Action kontrolü
        else if (linkController && linkAction) {
            const currentUrl = currentPath.split('/');
            if (currentUrl[1] === linkController && currentUrl[2] === linkAction) {
                link.classList.add('active');
            }
        }
        // Sadece Controller kontrolü
        else if (linkController) {
            const currentUrl = currentPath.split('/');
            if (currentUrl[1] === linkController) {
                link.classList.add('active');
            }
        }
    });

    // Pagination için JavaScript
    document.querySelectorAll('.pagination .page-link').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const pageUrl = this.getAttribute('href');
            window.location.href = pageUrl;
        });
    });

    // Açıklama alanı karakter sınırı kontrolü
    const descriptionTextareas = document.querySelectorAll('textarea[data-maxlength="150"]');
    descriptionTextareas.forEach(textarea => {
        textarea.addEventListener('input', function() {
            const warningDiv = this.parentNode.querySelector('#descriptionLimitWarning');
            if (this.value.length === 150) {
                warningDiv.style.display = 'block';
            } else {
                warningDiv.style.display = 'none';
            }
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    //  etiketleri göster/gizle düğmesi 
    var toggleButton = document.getElementById("toggleTagsBtn");
    var tagsSection = document.getElementById("allTagsSection");

    if (toggleButton && tagsSection) {
        toggleButton.addEventListener("click", function () {
            if (tagsSection.style.display === "block") {
                tagsSection.style.display = "none";
                toggleButton.textContent = "Tüm Etiketleri Göster";
            } else {
                tagsSection.style.display = "block";
                toggleButton.textContent = "Etiketleri Gizle";
            }
        });
    }

    // pagination için 
    document.querySelectorAll('.pagination .page-link').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const pageUrl = this.getAttribute('href');
            window.location.href = pageUrl;
        });
    });
});

// CKEditor entegrasyonu
ClassicEditor
    .create(document.querySelector('#editor'), {
        toolbar: ['heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote', 'insertTable', 'undo', 'redo'],
        heading: {
            options: [
                { model: 'paragraph', title: 'Paragraf', class: 'ck-heading_paragraph' },
                { model: 'heading2', view: 'h2', title: 'Başlık 2', class: 'ck-heading_heading2' },
                { model: 'heading3', view: 'h3', title: 'Başlık 3', class: 'ck-heading_heading3' }
            ]
        }
    })
    .then(editor => {
        // CKEditor'un dark temaya uygun stillerini eklemek için
        editor.editing.view.change(writer => {
            writer.setStyle('background-color', 'var(--dark-tertiary)', editor.editing.view.document.getRoot());
            writer.setStyle('color', 'var(--text-primary)', editor.editing.view.document.getRoot());
        });
    })
    .catch(error => {
        console.error(error);
    });

// Etiket giriş sistemi
document.querySelector('#tagsInput').addEventListener('input', function () {
    let tags = this.value.split(',').map(tag => tag.trim()).filter(tag => tag !== '');
    let tagsListHtml = '';

    tags.forEach(tag => {
        tagsListHtml += `<span class="badge bg-secondary me-1 mb-1">${tag}</span>`;
    });

    document.querySelector('#tagsList').innerHTML = tagsListHtml;
});

// Sayfa yüklendiğinde mevcut etiketleri göster
document.addEventListener('DOMContentLoaded', function () {
    const tagsInput = document.querySelector('#tagsInput');
    if (tagsInput && tagsInput.value) {
        let tags = tagsInput.value.split(',').map(tag => tag.trim()).filter(tag => tag !== '');
        let tagsListHtml = '';

        tags.forEach(tag => {
            tagsListHtml += `<span class="badge bg-secondary me-1 mb-1">${tag}</span>`;
        });

        document.querySelector('#tagsList').innerHTML = tagsListHtml;
    }
});

$(document).ready(function () {
    $("#btnYorumKayit").click(function () {
        var text = $("#Text").val().trim();
        if (!text) {
            alert("Lütfen bir yorum yazın.");
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '@Url.Action("AddComment", "Comment")',
            dataType: 'json',
            data: {
                PostId: $('#PostId').val(),
                Text: text
            },
            success: function (response) {
                if (response.success) {
                    var comment = response.comment;
                    var date = new Date(comment.publishedOn);
                    var avatarPath = comment.avatar ? `/img/users/${comment.avatar}` : '/img/users/default.jpg';

                    $("#comments").prepend(`
                                <div class="comment">
                                    <img src="${avatarPath}" class="comment-avatar" alt="${comment.username}">
                                    <div class="comment-body">
                                        <div class="comment-header">
                                            <span class="comment-name">${comment.username}</span>
                                            <span class="comment-date">${date.toLocaleDateString()}</span>
                                        </div>
                                        <p class="comment-text">${comment.text}</p>
                                    </div>
                                </div>
                            `);

                    $("#Text").val('');

                    var adet = parseInt($("#commentCount").text());
                    $("#commentCount").text(adet + 1);
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                alert("Yorum eklenirken bir hata oluştu: " + error);
            }
        });
        return false;
    });
});

    

// URL otomatik oluşturma
document.querySelector('#Title').addEventListener('blur', function () {
    let titleValue = this.value;
    let urlField = document.querySelector('#Url');

    // URL alanı boşsa ve başlık doldurulduysa
    if (urlField.value === '' && titleValue !== '') {
        // Türkçe karakterleri değiştir ve URL'ye uygun hale getir
        let url = titleValue
            .toLowerCase()
            .replace(/ı/g, 'i')
            .replace(/ğ/g, 'g')
            .replace(/ü/g, 'u')
            .replace(/ş/g, 's')
            .replace(/ç/g, 'c')
            .replace(/ö/g, 'o')
            .replace(/\s+/g, '-')
            .replace(/[^a-z0-9\-]/g, '');

        urlField.value = url;
    }
});

// Etiket giriş sistemi
document.querySelector('#tagsInput').addEventListener('input', function () {
    let tags = this.value.split(',').map(tag => tag.trim()).filter(tag => tag !== '');
    let tagsListHtml = '';

    tags.forEach(tag => {
        tagsListHtml += `<span class="badge bg-secondary me-1 mb-1">${tag}</span>`;
    });

    document.querySelector('#tagsList').innerHTML = tagsListHtml;
});

$(document).ready(function () {
    // Yorum silme işlemi
    $('.delete-comment').click(function () {
        var commentId = $(this).data('comment-id');
        var postId = $(this).data('post-id');

        if (confirm('Bu yorumu silmek istediğinizden emin misiniz?')) {
            $.ajax({
                url: '/Comment/Delete',
                type: 'POST',
                data: {
                    commentId: commentId,
                    postId: postId
                },
                success: function (response) {
                    if (response.success) {
                        $('#comment-' + commentId).fadeOut(300, function () {
                            $(this).remove();
                        });
                    } else {
                        alert('Yorum silinirken bir hata oluştu: ' + response.message);
                    }
                },
                error: function () {
                    alert('Yorum silinirken bir hata oluştu.');
                }
            });
        }
    });
});