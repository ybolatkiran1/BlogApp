# BlogApp - Proje Dokümantasyonu

**BlogApp**, ASP.NET Core MVC (.NET 9.0) ile geliştirilmiş, kullanıcıların blog yazıları yazabildiği, yorum yapabildiği, hesaplarını yönetebildiği modern bir blog platformudur. Güçlü güvenlik altyapısı, sade ve kullanıcı dostu arayüzü ile hem geliştiriciler hem de kullanıcılar için ideal bir çözümdür.

---

## 🚀 Temel Özellikler

### 👥 Kullanıcı Yönetimi
- Güvenli kullanıcı kayıt ve giriş sistemi
- `BCrypt.Net-Next` ile şifre hashleme
- Minimum 6 karakter şifre zorunluluğu
- Benzersiz kullanıcı adı ve e-posta kontrolü
- Cookie tabanlı oturum yönetimi

### 👤 Profil Yönetimi
- Ad, soyad, kullanıcı adı düzenleme
- Profil resmi yükleme/güncelleme
- E-posta değiştirme
- Şifre güncelleme

### 📝 Blog Yönetimi
- Başlık, açıklama, içerik ve görsel ile yazı oluşturma
- Yazı durum kontrolü (aktif/pasif)
- Otomatik yayın tarihi atama
- Zengin metin editörü desteği
- Blog yazıları güncelleme

### 📚 İçerik Organizasyonu
- Sayfalama sistemi 
- Kategoriye göre filtreleme
- Tarihe göre sıralama

### 💬 Yorum Sistemi
- Yorum yapma, düzenleme ve silme
- Yorum tarihi görüntüleme
- Kullanıcı bazlı yorum yönetimi

---

## 🔧 Teknik Altyapı

### Backend
- **ASP.NET Core MVC 9.0**
- Katmanlı mimari (Controller-Service-Repository)
- Dependency Injection
- Middleware kullanımı
- Custom route tanımlamaları

### Veritabanı
- **Entity Framework Core**
- SQLite veritabanı
- Code-First yaklaşımı
- Migration yönetimi
- Seed Data ile ilk çalışmada görülen veriler

### Güvenlik
- BCrypt.Net-Next ile şifre güvenliği
- Cookie tabanlı kimlik doğrulama
- CSRF koruması

### Frontend
- **Bootstrap 5.3.2**
- **jQuery 3.7.1**
- Responsive ve sade kullanıcı arayüzü
- Özel bileşenler:
  - Navbar
  - Blog kartları
  - Dinamik yorum sistemi
  - Kullanıcı dostu form yapısı

---

## 🔐 Güvenlik Detayları
- Cookie tabanlı oturum kontrolü
- Güvenli çıkış ve oturum temizleme işlemleri
- Şifre Hashleme ile güvenli veritabanı

---

## 🧪 Test Senaryoları

### ✅ Kullanıcı Testleri
- Kayıt
  - Geçerli bilgilerle başarı
  - Tekrarlanan kullanıcı adı kontrolü
  - E-posta formatı kontrolü
  - Şifre uzunluğu kontrolü
- Giriş
  - Doğru bilgilerle giriş
  - Yanlış şifre reddi
  - Kullanıcı bulunamadı senaryosu
- Profil
  - Bilgi güncelleme
  - Şifre değiştirme
  - Profil resmi değiştirme
  - E-posta değiştirme

### ✅ Blog İşlemleri
- Yazı oluşturma, düzenleme, silme
- Yazı durumu değiştirme (aktif/pasif)
- İçerik ve başlık uzunluğu doğrulama
- URL benzersizlik kontrolü
- Görsel format uygunluğu kontrolü

### ✅ Yorum İşlemleri
- Yorum ekleme, düzenleme, silme
- Yetki kontrolü: yalnızca post sahibi silebilir

---

## 📊 Veritabanı Yapısı

### `Users`
| Alan        | Tip           | Açıklama                          |
|-------------|---------------|-----------------------------------|
| UserId      | int (PK)      | Otomatik artan kimlik            |
| UserName    | string        | Benzersiz kullanıcı adı          |
| Name        | string        | Adı                              |
| Surname     | string        | Soyadı                           |
| Email       | string        | Benzersiz e-posta                |
| Password    | string        | Hashlenmiş şifre                 |
| Image       | string        | Profil resmi yolu                |

### `Posts`
| Alan        | Tip           | Açıklama                          |
|-------------|---------------|-----------------------------------|
| PostId      | int (PK)      | Yazı kimliği                     |
| Title       | string        | Başlık                           |
| Content     | string        | İçerik                           |
| Description | string        | Kısa açıklama                    |
| Url         | string        | SEO uyumlu bağlantı              |
| Image       | string        | Görsel yolu                      |
| IsActive    | bool          | Durum bilgisi                    |
| PublishedOn | datetime      | Yayınlanma tarihi                |
| UserId      | int (FK)      | Yazıyı yazan kullanıcı           |

### `Comments`
| Alan        | Tip           | Açıklama                          |
|-------------|---------------|-----------------------------------|
| CommentId   | int (PK)      | Yorum kimliği                    |
| Text        | string        | Yorum içeriği                    |
| PublishedOn | datetime      | Yorum tarihi                     |
| UserId      | int (FK)      | Yorumu yazan kullanıcı           |
| PostId      | int (FK)      | Yorumun ait olduğu yazı          |

---

## 📎 Notlar

- Projede ASP.NET Identity yerine özel kullanıcı yönetimi sistemi kullanılmıştır.
- Uygulama SQLite destekli olup, kolay taşınabilir yapıdadır.
- Responsive tasarım sayesinde mobil cihazlarda da uyumlu çalışır.
- Görseller `wwwroot/img/users/` ve `wwwroot/img/post/` dizinlerinde saklanır.
- Projeye yeni modüller (beğeni sistemi, etiket yönetimi, admin paneli) kolayca eklenebilir yapıdadır.

---

**Teşekkürler** 
Her türlü katkıya ve geri bildirime açığız. 🎉
