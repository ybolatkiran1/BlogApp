using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Entity;
using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DataAccessLayer.Data
{
    public static class SeedData
    {
        public static void TestVerileri(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<Context>();
            if (context != null)
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                if (!context.Tags.Any())
                {

                    context.Tags.AddRange(
                    new Tag { Text = "web programlama", Url = "web-programlama", Color = TagColors.primary },
                    new Tag { Text = "backend", Url = "backend", Color = TagColors.danger },
                    new Tag { Text = "frontend", Url = "frontend", Color = TagColors.secondary },
                    new Tag { Text = "game", Url = "game", Color = TagColors.success },
                    new Tag { Text = "fullstack", Url = "full-stack", Color = TagColors.warning }
                );
                    context.SaveChanges();
                }
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User
                        {
                            UserName = "ybolatkiran",
                            Image = "default.jpg",
                            Name = "Emre Bolatkıran",
                            Email = "ybolatkiran@info.com",
                            Password = "123456",
                        },
                            new User { UserName = "ybolatk", Image = "default.jpg", Name = "Yunus Bolatkıran", Email = "ybolatk@info.com", Password = "123456" }

                );
                    context.SaveChanges();
                }
                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(

                        new Post
                        {
                            Title = "Asp.Net Core MVC yapısı",
                            Content = @"Asp.Net Core MVC, modern web uygulamaları geliştirmek için güçlü ve esnek bir yapıdır. MVC (Model-View-Controller) deseni, uygulamanın iş mantığını, kullanıcı arayüzünü ve verileri birbirinden ayırarak daha düzenli ve sürdürülebilir bir yapı sunar.

Model katmanı, uygulamanın veri kısmını temsil eder. Genellikle veritabanıyla etkileşim kuran sınıflar burada yer alır. View katmanı, kullanıcının gördüğü arayüzdür. Razor motoru kullanılarak HTML çıktılar oluşturulur. Controller katmanı ise gelen istekleri işler, gerekli işlemleri yapar ve uygun View’a yönlendirme yapar.

ASP.NET Core, modüler yapısı sayesinde bağımlılıkları azaltır ve performansı artırır. Middleware bileşenleri ile istek ve yanıtlar üzerinde özelleştirme yapılabilir. Dependency Injection (bağımlılık enjeksiyonu) doğrudan entegredir, bu da kodun test edilebilirliğini artırır.

Geliştiriciler için geniş bir kütüphane desteği, güçlü güvenlik altyapısı, kolay yapılandırma ve platform bağımsızlık gibi avantajlar sunar. Hem küçük hem de büyük ölçekli projelerde kullanılabilir."
,
                            Description = "Asp.Net Core MVC yapısında mimarisi",
                            Url = "asp-net-core",
                            IsActive = true,
                            Image = "4.jpg",
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(2).ToList(),
                            UserId = 1
                        },
                        new Post
                        {
                            Title = "Web Geliştirme Bootcamp",
                            Content = @"Web Geliştirme Bootcamp programı, sıfırdan profesyonel seviyeye kadar web geliştirme becerileri kazandırmayı hedefleyen yoğun ve uygulamalı bir eğitim sürecidir. Bu eğitim programı, katılımcıların HTML, CSS, JavaScript gibi temel teknolojilerden başlayarak, modern web geliştirme araçlarına kadar geniş bir yelpazede bilgi sahibi olmalarını sağlar.

Bootcamp’te ilk aşamada katılımcılar web sayfalarının yapısını oluşturmayı, CSS ile bu sayfalara stil vermeyi ve JavaScript ile interaktif özellikler kazandırmayı öğrenirler. Daha sonra React.js gibi modern front-end kütüphaneleri ve ASP.NET Core gibi back-end teknolojileri devreye girer.

Veri yönetimi için SQL ve NoSQL veritabanları, versiyon kontrolü için Git, proje yönetimi için Trello ve GitHub gibi araçlar tanıtılır. Eğitimin son bölümünde ekip çalışmaları ve proje bazlı uygulamalar yapılır. Katılımcılar, kendi portföylerini oluşturabilecekleri gerçek dünya projeleri geliştirirler.

Bu eğitim, yazılım dünyasına hızlı bir giriş yapmak ve sektörde kariyer hedefleyenler için oldukça güçlü bir başlangıç noktasıdır."
,
                            Description = "Web geliştirme ile ilgili ders içerikleri.",
                            Url = "web-gelistirme-bootcamp",
                            IsActive = true,
                            Image = "5.jpg",
                            PublishedOn = DateTime.Now.AddDays(-15),
                            Tags = context.Tags.Take(2).ToList(),
                            UserId = 1
                        },
                        new Post
                        {
                            Title = "Mobil Uygulama Geliştirme Eğitimi",
                            Content = @"Mobil uygulamalar hayatımızın vazgeçilmez bir parçası haline geldi. Mobil Uygulama Geliştirme Eğitimi, hem iOS hem de Android platformlarında çalışabilecek modern mobil uygulamalar geliştirmeyi hedefleyen kapsamlı bir eğitimdir.

Eğitimde ilk olarak mobil uygulama geliştirme ekosistemi, platform farkları ve temel kavramlar anlatılır. Ardından Flutter ve React Native gibi çapraz platform geliştirme araçları tanıtılır. Bu araçlar sayesinde tek bir kod tabanıyla hem iOS hem de Android için uygulama geliştirmek mümkündür.

Katılımcılar, kullanıcı arayüzü tasarımı, navigasyon, veri saklama yöntemleri, API ile veri çekme ve kullanıcı etkileşimleri konularında detaylı bilgi edinirler. Firebase, SQLite, RESTful API gibi teknolojiler aktif olarak kullanılır.

Eğitim sonunda katılımcılar, kendi mobil uygulamalarını mağazalara yükleyebilir, freelance projeler yapabilir ya da kurumsal projelere katkı sağlayabilecek yeterliliğe ulaşırlar."
,
                            Description = "Mobil uygulama geliştirme konusundaki ders içerikleri.",
                            Url = "mobil-uygulama-gelistirme-egitimi",
                            IsActive = true,
                            Image = "6.jpg",
                            PublishedOn = DateTime.Now.AddDays(-15),
                            Tags = context.Tags.Take(3).ToList(),
                            UserId = 1
                        },
                        new Post
                        {
                            Title = "Veri Bilimi ve Yapay Zeka Bootcamp",
                            Content = @"Veri Bilimi ve Yapay Zeka Bootcamp’i, geleceğin en önemli meslek gruplarından biri olan veri bilimciliğe adım atmak isteyenler için özel olarak hazırlanmış bir eğitim programıdır. Bu program sayesinde katılımcılar hem teorik hem de pratik anlamda güçlü bir temel oluşturur.

Eğitimde ilk olarak veri analizi ve istatistiksel yaklaşımlar öğretilir. Python dili üzerinden NumPy, Pandas, Matplotlib ve Seaborn gibi kütüphanelerle veri işleme ve görselleştirme teknikleri anlatılır. Ardından makine öğrenmesi algoritmalarına geçilir.

Makine öğrenmesi bölümü, gözetimli ve gözetimsiz öğrenme algoritmalarını kapsar. Sınıflandırma, regresyon, kümeleme gibi tekniklerle farklı veri setleri üzerinde uygulamalar yapılır. Son olarak derin öğrenme konularına geçilerek TensorFlow ve Keras gibi framework’ler ile yapay sinir ağları modellenir.

Gerçek dünya problemlerinin çözüldüğü proje bazlı uygulamalar sayesinde katılımcılar, portföylerine güçlü projeler eklerler ve veri odaklı kariyer hedeflerine sağlam adımlarla ilerlerler."
,
                            Description = "Veri bilimi ve yapay zeka ders içerikleri.",
                            Url = "veri-bilimi-ve-yapay-zeka-bootcamp",
                            IsActive = true,
                            Image = "7.jpg",
                            PublishedOn = DateTime.Now.AddDays(-35),
                            Tags = context.Tags.Take(4).ToList(),
                            UserId = 1
                        },
                        new Post
                        {
                            Title = "Siber Güvenlik Eğitim Programı",
                            Content = @"Dijital dünyadaki tehditlerin artmasıyla birlikte siber güvenlik, artık her birey ve kurum için bir öncelik haline geldi. Siber Güvenlik Eğitim Programı, bu alanda kariyer yapmak isteyenler veya mevcut bilgi birikimini geliştirmek isteyenler için özel olarak tasarlanmıştır.

Eğitim sürecinde siber güvenliğin temelleri, ağ güvenliği, sistem güvenliği, saldırı türleri ve savunma yöntemleri detaylı bir şekilde ele alınır. Katılımcılar; firewall, antivirüs sistemleri, IDS/IPS, DDoS koruma sistemleri gibi teknolojileri öğrenir.

Penetrasyon testleri, etik hackerlık yöntemleri, güvenlik açıkları tespiti ve exploit geliştirme gibi ileri düzey konular da uygulamalı olarak işlenir. Ayrıca siber güvenlikte kullanılan araçlar (Wireshark, Metasploit, Burp Suite, Nmap) aktif olarak kullanılır.

Bu eğitim sonunda katılımcılar, güvenli sistemler tasarlayabilir, zafiyet analizi yapabilir ve siber tehditlere karşı önlem alabilecek düzeyde bilgi sahibi olurlar."
,
                            Description = "Siber güvenlik üzerine ders içerikleri.",
                            Url = "siber-guvenlik-egitim-programi",
                            IsActive = true,
                            Image = "2.jpg",
                            PublishedOn = DateTime.Now.AddDays(-5),
                            Tags = context.Tags.Take(1).ToList(),
                            UserId = 1
                        },
                        new Post
                        {
                            Title = "Yazılım Geliştirici Sertifika Programı",
                            Content = "Yazılım Geliştirme Sertifikalı Eğitimleri Başlıyor\r\nTeknolojinin hızla ilerlediği günümüzde, yazılım geliştirme alanı en çok talep gören ve en dinamik sektörlerden biri haline geldi. Dijitalleşmenin her alana yayılmasıyla birlikte, yazılım geliştiricilere olan ihtiyaç da her geçen gün artıyor. Bu kapsamda başlatılan Yazılım Geliştirme Sertifikalı Eğitim Programı, sektöre sağlam bir giriş yapmak isteyen bireyler için eşsiz bir fırsat sunuyor.\r\n\r\nNeden Sertifikalı Eğitim?\r\nYazılım geliştiricilik, sadece kod yazmayı değil; aynı zamanda problem çözmeyi, algoritmik düşünmeyi, ekip çalışmasına uyum sağlamayı ve sürekli öğrenmeyi gerektirir. Bu program, katılımcılara yalnızca teknik bilgi kazandırmakla kalmaz, aynı zamanda sektörde geçerli olan bir sertifika ile bilgi birikimini belgelendirme fırsatı sunar. Bu sayede işe alım süreçlerinde bir adım öne geçmek mümkün hale gelir.\r\n\r\nEğitimde Neler Var?\r\nBu kapsamlı eğitim programı, yazılım geliştirmenin temellerinden başlayarak ileri düzey konulara kadar uzanan geniş bir yelpazeyi kapsar. Eğitim içeriğinde yer alan bazı başlıklar şunlardır:\r\n\r\nProgramlama Temelleri: Değişkenler, döngüler, koşullar, veri tipleri, fonksiyonlar\r\n\r\nNesne Yönelimli Programlama (OOP)\r\n\r\nVeritabanı Yönetimi: SQL ve ilişkisel veritabanı mantığı\r\n\r\nWeb Programlama: HTML, CSS, JavaScript, React.js gibi modern web teknolojileri\r\n\r\nBack-End Geliştirme: ASP.NET Core MVC, Node.js gibi teknolojilerle sunucu taraflı yazılım geliştirme\r\n\r\nAPI Geliştirme ve Entegrasyon\r\n\r\nVersiyon Kontrol Sistemleri: Git ve GitHub kullanımı\r\n\r\nYazılım Testleri ve Temiz Kod Yazımı\r\n\r\n",
                            Description = "Yazılım geliştirme sertifika programı içerikleri.",
                            Url = "yazilim-gelistirici-sertifika-programi",
                            IsActive = true,
                            Image = "8.jpg",
                            PublishedOn = DateTime.Now.AddDays(-25),
                            Tags = context.Tags.Take(3).ToList(),
                            UserId = 1
                        },
                        new Post
                        {
                            Title = "JavaScript ve Front-End Bootcamp",
                            Content = "JavaScript ve Front-End Eğitimi Devam Ediyor\r\nFront-End dünyası, her geçen gün gelişmeye ve dönüşmeye devam ediyor. Kullanıcı deneyimini merkeze alan bu alanda, özellikle JavaScript'in önemi yadsınamaz. Modern web uygulamalarının temel taşı haline gelen JavaScript, yalnızca etkileşimli sayfalar oluşturmakla kalmıyor, aynı zamanda tüm kullanıcı arayüzlerinin bel kemiğini oluşturuyor.\r\n\r\nNeden JavaScript?\r\nJavaScript, HTML ve CSS ile birlikte web’in 3 temel teknolojisinden biridir. Tarayıcıda çalışan bir programlama dili olması sayesinde, kullanıcılarla doğrudan etkileşim kurabilen arayüzler tasarlamak mümkündür. Üstelik bu dil yalnızca basit animasyonlar veya buton işlevleriyle sınırlı değil; API entegrasyonları, veri işleme, form validasyonları ve hatta oyunlar geliştirmek için bile kullanılabiliyor.\r\n\r\nFront-End Framework ve Kütüphaneler\r\nJavaScript’in gücünü daha etkili kullanmak için birçok framework ve kütüphane geliştirildi. Eğitimde özellikle şu konulara ağırlık veriliyor:\r\n\r\nReact.js: Facebook tarafından geliştirilen bu kütüphane, bileşen tabanlı mimarisiyle en çok tercih edilen Front-End araçlarından biri. Reaktif ve yeniden kullanılabilir bileşenler sayesinde büyük projelerde bile düzenli bir yapı sağlar.\r\n\r\nVue.js: Öğrenmesi kolay, kullanımı pratik olan bu framework, özellikle küçük ve orta ölçekli projeler için idealdir.\r\n\r\nBootstrap ve Tailwind CSS: Arayüz geliştirmeyi hızlandıran bu araçlar sayesinde duyarlı ve modern tasarımlar üretmek oldukça kolay hale geliyor.\r\n\r\nEğitimde Neler Öğreniliyor?\r\nEğitim boyunca katılımcılar sadece kod yazmakla kalmıyor, aynı zamanda yazılım geliştirme süreçlerinin temellerini de öğreniyorlar. Özellikle proje bazlı ilerleyen derslerde:\r\n\r\nHTML5 ve semantik etiketler\r\n\r\nCSS3 ile stillendirme ve layout teknikleri\r\n\r\nFlexbox, Grid yapıları\r\n\r\nJavaScript temelleri (değişkenler, döngüler, koşullar, fonksiyonlar)\r\n\r\nDOM Manipülasyonu\r\n\r\nEvent Handling (Olay Yönetimi)\r\n\r\nAsenkron programlama (Promises, async/await)\r\n\r\nAPI kullanımı ve veri çekme\r\n\r\nWeb performansı ve SEO dostu yapılar\r\n\r\ngibi konular ele alınıyor.\r\n\r\nProje Odaklı Yaklaşım\r\nTeorik bilginin yanı sıra uygulamalı projelerle öğrencilerin kendilerini geliştirmesi hedefleniyor. Katılımcılar, portföylerine ekleyebilecekleri çeşitli projeler geliştiriyor. Bu projeler arasında:\r\n\r\nKişisel blog sayfası\r\n\r\nHava durumu uygulaması\r\n\r\nTo-do list uygulaması\r\n\r\nFilm arama motoru (OMDb API entegrasyonlu)\r\n\r\nBasit bir e-ticaret arayüzü\r\n\r\ngibi çalışmalar yer alıyor.\r\n\r\nEğitim Sonunda Ne Hedefleniyor?\r\nBu eğitim sayesinde katılımcılar, sıfırdan kendi web sitelerini tasarlayabilir hale geliyor. Ayrıca bir Front-End geliştirici olarak freelance işler alabilecek düzeye gelebiliyorlar. Aynı zamanda, Full-Stack Developer yolculuğunun da temelini oluşturmuş oluyorlar.",
                            Description = "JavaScript ve Front-End ders içerikleri.",
                            Url = "javascript-ve-front-end-bootcamp",
                            IsActive = true,
                            Image = "9.jpg",
                            PublishedOn = DateTime.Now.AddDays(-5),
                            Tags = context.Tags.Take(2).ToList(),
                            UserId = 1
                        },
                         new Post
                         {
                             Title = "C# ile Nesne Yönelimli Programlama",
                             Content = @"Nesne Yönelimli Programlama (OOP), yazılım geliştirme sürecinde kodun daha modüler, okunabilir ve sürdürülebilir olmasını sağlayan bir paradigmadır. C# dili, OOP prensiplerini tam anlamıyla destekleyen modern bir dildir. Bu yazıda, C# ile OOP’nin temel kavramlarına değineceğiz: sınıflar, nesneler, kalıtım, çok biçimlilik, kapsülleme ve soyutlama.

Sınıflar, bir nesnenin şablonunu oluşturur. Örneğin bir 'Araba' sınıfı oluşturup, renk, model ve hız gibi özellikleri tanımlayabiliriz. Daha sonra bu sınıftan birçok farklı 'Araba' nesnesi türetilebilir.

Kalıtım, bir sınıfın başka bir sınıfın özelliklerini devralmasına olanak tanır. Bu sayede tekrar eden kodlar azaltılır. Örneğin 'Tasit' sınıfı, 'Araba' ve 'Otobüs' gibi alt sınıflar tarafından miras alınabilir.

Çok biçimlilik sayesinde bir nesne, birden fazla formda davranabilir. Özellikle arayüzler ve sanal (virtual) metotlar ile bu esneklik sağlanır.

Kapsülleme ile sınıfın iç yapısı gizlenerek, dış dünyaya yalnızca gerekli bilgiler sunulur. Bu, verilerin güvenliğini artırır.

Son olarak soyutlama, karmaşık sistemleri basitleştirerek sadece gerekli bilgileri öne çıkarmaya yarar.

C#’ta OOP uygulamaları ile büyük ölçekli yazılımlar daha güvenli ve sürdürülebilir hale getirilebilir.",
                             Description = "C# dilinde OOP kavramları detaylı şekilde anlatılmaktadır.",
                             Url = "csharp-nesne-yonelimli-programlama",
                             IsActive = true,
                             Image = "10.jpg",
                             PublishedOn = DateTime.Now.AddDays(-12),
                             Tags = context.Tags.Skip(1).Take(2).ToList(),
                             UserId = 2
                         },
    new Post
    {
        Title = "Python ile Veri Analizi",
        Content = @"Python, veri bilimi ve analiz alanında en çok tercih edilen programlama dillerinden biridir. Özellikle Pandas, NumPy, Matplotlib ve Seaborn gibi güçlü kütüphaneleri ile veri analizi süreçlerini oldukça kolaylaştırır. Bu yazıda, veri analizi için temel adımları inceleyeceğiz.

İlk adım verilerin yüklenmesidir. Genellikle CSV formatındaki dosyalar `pandas.read_csv()` fonksiyonu ile okunur. Ardından verilerin yapısı incelenerek eksik veriler, aykırı değerler ve veri tipleri analiz edilir.

Veri temizleme aşamasında, eksik veriler doldurulur veya silinir. Gerekirse veri tipi dönüşümleri yapılır. Bu işlem, analizlerin doğruluğu için kritik öneme sahiptir.

Veri görselleştirme ise verileri daha iyi anlamamıza yardımcı olur. Matplotlib ve Seaborn kütüphaneleri ile histogram, bar chart, scatter plot gibi grafikler oluşturulabilir. Özellikle korelasyon matrisleri, değişkenler arasındaki ilişkileri analiz etmekte faydalıdır.

Son olarak istatistiksel analiz ve modelleme adımları ile veriden anlamlı sonuçlar çıkarılır. Python, tüm bu adımlar için zengin bir ekosisteme sahiptir ve öğrenmesi oldukça kolaydır.",
        Description = "Pandas ve NumPy kullanarak veri analizi.",
        Url = "python-veri-analizi",
        IsActive = true,
        Image = "11.jpg",
        PublishedOn = DateTime.Now.AddDays(-22),
        Tags = context.Tags.Take(3).ToList(),
        UserId = 1
    },
    new Post
    {
        Title = "React ile Modern Web Uygulamaları",
        Content = @"React, Facebook tarafından geliştirilen ve günümüzde en çok kullanılan JavaScript kütüphanelerinden biridir. React, component (bileşen) tabanlı yapısıyla, büyük ve karmaşık web uygulamalarının yönetimini kolaylaştırır.

React'in temelinde JSX (JavaScript XML) yer alır. JSX, HTML benzeri sözdizimini JavaScript içinde yazmamıza olanak tanır. Bu yapı sayesinde UI bileşenlerini modüler olarak tanımlayıp tekrar kullanılabilir hale getirebiliriz.

React’te her şey bir bileşendir. Bileşenler ya sınıf (class component) ya da fonksiyon (function component) olarak tanımlanabilir. Günümüzde genellikle `useState`, `useEffect` gibi hook’lar ile fonksiyonel bileşenler tercih edilmektedir.

State yönetimi React'te önemli bir konudur. Bileşenlerin durumlarını yönetmek için `useState`, global state yönetimi için ise Redux veya Context API gibi yapılar kullanılır.

React Router ile SPA (Single Page Application) mimarisi kurulabilir. Bu sayede sayfa yenilenmeden dinamik rotalar arasında geçiş yapılabilir.

React sayesinde, performansı yüksek, ölçeklenebilir ve kullanıcı dostu modern web uygulamaları geliştirmek mümkündür.",
        Description = "React.js ile component tabanlı frontend geliştirme.",
        Url = "react-modern-web",
        IsActive = true,
        Image = "12.jpg",
        PublishedOn = DateTime.Now.AddDays(-18),
        Tags = context.Tags.Skip(2).Take(2).ToList(),
        UserId = 2
    },
    new Post
    {
        Title = "SQL Temelleri ve Veritabanı Yönetimi",
        Content = @"SQL (Structured Query Language), ilişkisel veritabanları ile iletişim kurmak için kullanılan standart bir dildir. MySQL, PostgreSQL, SQL Server gibi veritabanı yönetim sistemlerinde ortak olarak kullanılan bu dil, veri çekme, güncelleme, silme ve ekleme işlemlerinin temelini oluşturur.

Bir veritabanında en temel yapı tablo (table)’dur. Tablo, satır (row) ve sütunlardan (column) oluşur. `SELECT` komutu, tablodan veri çekmek için kullanılırken, `INSERT`, `UPDATE` ve `DELETE` komutları sırasıyla veri ekleme, güncelleme ve silme işlemleri için kullanılır.

WHERE, JOIN, GROUP BY, ORDER BY gibi ifadeler, sorgulara koşul ve filtreler eklememizi sağlar. Özellikle `JOIN` yapıları, birden fazla tablonun birleştirilerek ortak verilerin çekilmesini sağlar.

SQL dilinin iyi öğrenilmesi, veri ile çalışan her geliştirici için büyük avantaj sağlar. Veri tabanı optimizasyonu, index kullanımı ve normalizasyon gibi ileri seviye konular ise sistem performansını doğrudan etkiler.",
        Description = "SQL dili ve temel veritabanı işlemleri.",
        Url = "sql-temelleri",
        IsActive = true,
        Image = "13.jpg",
        PublishedOn = DateTime.Now.AddDays(-30),
        Tags = context.Tags.Take(1).ToList(),
        UserId = 2
    }
                        );
                    context.SaveChanges();
                }
            }
        }
    }
}
