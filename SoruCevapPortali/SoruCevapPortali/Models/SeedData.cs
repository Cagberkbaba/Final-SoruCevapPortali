using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;

namespace SoruCevapPortali.Models
{
    public class SeedData
    {
        public static async Task IdentitySeeddata(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.GetAppliedMigrations().Any()) // bekleyen migration varsa onu veritabanına ekler
            {
                context.Database.Migrate();
            }
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryName = "Teknoloji" },
                    new Category { CategoryName = "Tarih" },
                    new Category { CategoryName = "Siyaset" },
                    new Category { CategoryName = "Bilim" },
                    new Category { CategoryName = "Kültür" },
                    new Category { CategoryName = "Spor" },
                    new Category { CategoryName = "Eğlence" },
                    new Category { CategoryName = "Gündelik" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }


            // Veritabanında Questions tablosu boş mu kontrol et
            if (!context.Questions.Any())
            {
                var categoriesList = await context.Categories.ToListAsync();

                var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var userList = await userManager.Users.ToListAsync();
                if (userList == null || !userList.Any())
                {
                    var password = "Abc.123";
                    var users = new List<AppUser>
                    {
                        new AppUser { UserName = "HakanSen" ,FullName = "Hakan Şen",CreatedAt = new DateTime(2024, 9, 1, 10, 30, 45),Email = "hakansen@gmail.com"},
                        new AppUser { UserName = "Selmanmutlu" ,FullName = "Selman Mutlu",CreatedAt = new DateTime(2024, 10, 5, 10, 30, 45),Email = "selman@gmail.com"},
                        new AppUser { UserName = "HusnuColak" ,FullName = "Hüsnü Çolak",CreatedAt = new DateTime(2024, 11, 6, 10, 30, 45),Email = "husnu@gmail.com"},
                        new AppUser { UserName = "EmreKoc" ,FullName = "Emre Koç",CreatedAt = new DateTime(2024, 12, 1, 10, 00, 45),Email = "emrekoc@gmail.com"},
                         new AppUser { UserName = "DenizEkin" ,FullName = "Deniz Ekin",CreatedAt = new DateTime(2024, 5, 1, 10, 00, 45),Email = "denizekin@gmail.com"},
                          new AppUser { UserName = "SelmaSensoy" ,FullName = "Selma Şensoy",CreatedAt = new DateTime(2024, 8, 1, 11, 00, 45),Email = "selmasensoy@gmail.com"},
                         new AppUser { UserName = "Admin" ,FullName = "Admin",CreatedAt = new DateTime(2024, 12, 1, 10, 00, 45),Email = "Admin@gmail.com"},
                    };
                    foreach (var user in users)
                    {
                        await userManager.CreateAsync(user, password);


                        if (user.UserName == "Admin")
                        {
                            var roleExist = await roleManager.RoleExistsAsync("Admin");
                            if (!roleExist)
                            {
                                var role = new AppRole { Name = "Admin" };
                                await roleManager.CreateAsync(role);
                                await userManager.AddToRoleAsync(user, "Admin");
                            }
                        }
                        else
                        {
                            var roleExist = await roleManager.RoleExistsAsync("Uye");
                            if (!roleExist)
                            {
                                var role = new AppRole { Name = "Uye" };
                                await roleManager.CreateAsync(role);
                                await userManager.AddToRoleAsync(user, "Uye");
                            }
                        }

                    }
                    await context.SaveChangesAsync();
                }

                userList = await userManager.Users.ToListAsync();




           

                var questions = new List<Question>
            {
                // Teknoloji Kategorisi
                new Question
                {
                    QuestionTitle = "Teknolojinin hayatımızdaki rolü hakkında ne düşünüyorsunuz?",
                    QuestionText = "Teknolojinin hızla gelişmesi, yaşam biçimimizi her geçen gün daha fazla değiştirmekte. Teknolojinin pozitif ve negatif etkileri hakkında ne düşünüyorsunuz? Teknolojiyle ilgili kişisel deneyimleriniz nelerdir?",
                    CategoryId =  categoriesList[0].CategoryId ,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Question
                {
                    QuestionTitle = "En sevdiğiniz teknoloji ürünü nedir ve neden?",
                    QuestionText = "Teknoloji hayatımızın vazgeçilmez bir parçası haline geldi. En sevdiğiniz teknoloji ürününü ve bunun sizin günlük yaşamınızı nasıl etkilediğini anlatır mısınız?",
                    CategoryId = categoriesList[0].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-100)
                },
                new Question
                {
                    QuestionTitle = "Teknolojik yeniliklerin toplumu nasıl dönüştürdüğünü düşünüyorsunuz?",
                    QuestionText = "Yeni teknolojik gelişmelerin toplumsal yapıyı, iş gücünü, eğitim sistemini ve kişisel yaşamları nasıl etkilediğini gözlemliyorsunuz? Bu değişikliklerin uzun vadede hangi sonuçları doğuracağını öngörüyorsunuz?",
                    CategoryId = categoriesList[0].CategoryId,AppUserId ="c026e6a3-3553-43a7-83be-5be269d7e526",CreatedAt = DateTime.Now.AddDays(-20)
                },

                // Tarih Kategorisi
                new Question
                {
                    QuestionTitle = "Tarihi bir olayın günümüzdeki etkileri hakkında ne düşünüyorsunuz?",
                    QuestionText = "Geçmişte yaşanan büyük olaylar, günümüz toplumlarını hala etkilemektedir. Sizce, tarihi bir olayın bugün nasıl bir etkisi olabilir? Hangi tarihi olayların günümüze kadar etkileri sürmektedir?",
                    CategoryId = categoriesList[1].CategoryId,AppUserId ="c026e6a3-3553-43a7-83be-5be269d7e526",CreatedAt = DateTime.Now.AddDays(-25)
                },
                new Question
                {
                    QuestionTitle = "Tarih boyunca en çok etkilendiğiniz lider kimdir?",
                    QuestionText = "Tarihteki liderlerin kararları ve vizyonları toplumları nasıl şekillendirmiştir? En çok etkilendiğiniz lideri seçerek, onun kararlarının toplum üzerindeki etkilerini anlatır mısınız?",
                    CategoryId = categoriesList[1].CategoryId, AppUserId ="c026e6a3-3553-43a7-83be-5be269d7e526",CreatedAt = DateTime.Now.AddDays(-30)
                },
               

                // Siyaset Kategorisi
                new Question
                {
                    QuestionTitle = "Siyasi ideolojilerin toplum üzerindeki etkilerini nasıl değerlendiriyorsunuz?",
                    QuestionText = "Siyasi ideolojiler, bir toplumun ekonomik, sosyal ve kültürel yapısını şekillendiren güçlü araçlardır. Farklı ideolojilerin toplumlar üzerinde nasıl etkiler yarattığını düşünüyorsunuz ve günümüzde bu ideolojiler ne gibi zorluklarla karşılaşıyor?",
                    CategoryId = categoriesList[2].CategoryId, AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-17)
                },
                new Question
                {
                    QuestionTitle = "Bir ülkenin siyasi yapısının toplum üzerindeki etkileri nelerdir?",
                    QuestionText = "Bir ülkenin demokratik, otoriter ya da monarşik siyasi yapısı, o toplumun bireylerinin yaşam tarzlarını nasıl etkiler? Toplumda özgürlük, eşitlik ve adalet gibi temel kavramların varlığı, o ülkenin siyasi yapısına nasıl yansır?",
                    CategoryId = categoriesList[2].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-21)
                },
               

                // Bilim Kategorisi
                new Question
                {
                    QuestionTitle = "Bilimsel keşiflerin günümüz toplumlarına etkileri hakkında ne düşünüyorsunuz?",
                    QuestionText = "Bilimsel keşifler, sağlık, teknoloji ve diğer birçok alanda önemli değişikliklere yol açmıştır. Bu bilimsel keşiflerin toplumları nasıl dönüştürdüğünü ve bireylerin yaşamını nasıl iyileştirdiğini anlatır mısınız?",
                    CategoryId = categoriesList[3].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-19)
                },

                new Question
                {
                    QuestionTitle = "Bilimsel araştırmaların toplumun geleceğini şekillendirmedeki rolü nedir?",
                    QuestionText = "Bilimsel araştırmalar, toplumların gelişmesine katkı sağlamakta önemli bir rol oynar. Bilimsel buluşlar ve teoriler, toplumların geleceğini nasıl şekillendiriyor? Gelecekte bilimsel gelişmelerin toplumsal yapıları nasıl dönüştürebileceğini düşünüyorsunuz?",
                    CategoryId = categoriesList[3].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-23)
                },

                // Kültür Kategorisi
                new Question
                {
                    QuestionTitle = "Farklı kültürlerin etkileşimi hakkında ne düşünüyorsunuz?",
                    QuestionText = "Kültürler arası etkileşim, özellikle küreselleşme ile birlikte giderek daha fazla artıyor. Farklı kültürlerin birbirini nasıl etkilediğini ve bu etkileşimin toplumsal yapılar üzerindeki etkilerini nasıl görüyorsunuz?",
                    CategoryId = categoriesList[4].CategoryId,AppUserId ="f34759fe-774b-454e-a5ab-822a263faad3",CreatedAt = DateTime.Now.AddDays(-21),
                },
                new Question
                {
                    QuestionTitle = "Kültürel mirasın korunması toplumlar için neden önemlidir?",
                    QuestionText = "Kültürel miras, bir toplumun geçmişini ve değerlerini yansıtan önemli bir unsurdur. Kültürel mirasın korunması, toplumsal kimlik ve tarihsel bağlam açısından büyük önem taşır. Kültürel mirasın korunması için neler yapılmalıdır?",
                    CategoryId = categoriesList[4].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-31)
                },
                new Question
                {
                    QuestionTitle = "Kültürel değerler günümüzde ne kadar önemli ve nasıl korunmalıdır?",
                    QuestionText = "Kültürel değerler, toplumların hayatında önemli bir yer tutar. Ancak modernleşme ile birlikte bazı kültürel değerlerin kaybolma riski vardır. Bu değerlerin korunması için neler yapılabilir? Kültürel değerlerin korunmasının toplumsal önemi nedir?",
                    CategoryId = categoriesList[4].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-28)
                },

                // Spor Kategorisi
                new Question
                {
                    QuestionTitle = "Sporun bireylerin fiziksel ve zihinsel sağlığına etkisi nedir?",
                    QuestionText = "Spor, sadece fiziksel sağlık değil, aynı zamanda zihinsel sağlık açısından da önemli faydalar sağlar. Düzenli spor yapmanın, insanların fiziksel sağlığı üzerinde nasıl etkiler yarattığını ve psikolojik durumlarını nasıl iyileştirdiğini anlatabilir misiniz?",
                    CategoryId = categoriesList[5].CategoryId,AppUserId ="f34759fe-774b-454e-a5ab-822a263faad3",CreatedAt = DateTime.Now.AddDays(-17)
                },
                new Question
                {
                    QuestionTitle = "Sporun, toplumun sosyal yapısındaki rolü nedir?",
                    QuestionText = "Spor, sadece fiziksel bir etkinlik olmanın ötesinde, bir toplumun sosyal yapısının şekillenmesinde de önemli bir rol oynar. Sporun toplumları birleştirici etkilerini ve insanlar arasındaki bağları nasıl güçlendirdiğini düşünüyorsunuz?",
                    CategoryId = categoriesList[5].CategoryId,AppUserId ="b1eb8555-e66c-4c65-8eed-abb0824d5796",CreatedAt = DateTime.Now.AddDays(-19)
                }
            };

                foreach (var q in questions)
                {
                    Random random = new Random();
                    int UserNumber = random.Next(0, 3);
                    q.AppUserId = userList[UserNumber].Id;
                    await context.Questions.AddAsync(q);
                }
                //Verileri kaydet
                await context.SaveChangesAsync();
            }

            if (!context.Answers.Any())
            {
                var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var userList = await userManager.Users.ToListAsync();
                // cevap formatı 
                var questions = await context.Questions.ToListAsync();

                var answers = new List<Answer>
{
    // **Teknoloji Sorusu ve Cevapları** (3 Cevap)
    new Answer { AnswerText = "Teknolojinin hızla gelişmesi, hayatımızın her alanına dokunuyor. Özellikle yapay zeka ve internet teknolojileri, iş dünyasında devrim yaratıyor. Bu yeniliklerin sağlık ve eğitim gibi alanlara etkisi ise oldukça belirgin.", AppUserId = "f34759fe-774b-454e-a5ab-822a263faad3", QuestionId = 2 },
    new Answer { AnswerText = "Teknoloji, iletişimde devrim yarattı. Artık insanlar dünyanın her yerinden anında birbirleriyle iletişim kurabiliyor. Bu, özellikle iş dünyasında küresel çapta iş yapmayı kolaylaştırıyor.", AppUserId = "c026e6a3-3553-43a7-83be-5be269d7e526", QuestionId = 2 ,CreatedAt = DateTime.Now.AddDays(-10)},
    
    // **Tarih Sorusu ve Cevapları** (2 Cevap)
    new Answer { AnswerText = "Tarihteki önemli liderlerin aldıkları kararlar, sadece kendi dönemlerine değil, sonraki nesillere de etki etmiştir. Örneğin, II. Dünya Savaşı sonrasındaki barış antlaşmaları, dünyadaki siyasi düzeni değiştirdi.", AppUserId = "b1eb8555-e66c-4c65-8eed-abb0824d5796", QuestionId = 6 },
    new Answer { AnswerText = "Tarihteki büyük olaylar, her zaman günümüze etki eder. Fransız Devrimi'nin getirdiği özgürlük anlayışı, bugünkü demokrasi sistemlerinin temellerini atmıştır. Bu gibi değişimler, toplumların yapısını temelden değiştiriyor.", AppUserId = "f34759fe-774b-454e-a5ab-822a263faad3", QuestionId = 5 ,CreatedAt = DateTime.Now.AddDays(-8)},

    // **Siyaset Sorusu ve Cevapları** (4 Cevap)
    new Answer { AnswerText = "Siyasi ideolojiler, devlet yönetim biçimlerini etkiler. Örneğin, sosyalizm, eşitlikçi bir toplum yapısının ortaya çıkmasını savunurken, kapitalizm daha çok bireysel özgürlük ve serbest piyasa ekonomisi üzerine yoğunlaşır.", AppUserId = "b1eb8555-e66c-4c65-8eed-abb0824d5796", QuestionId = 7,CreatedAt = DateTime.Now.AddDays(-5) },
    new Answer { AnswerText = "Siyasi düşünceler, toplumların yöneticilerini ve yönetim biçimlerini şekillendirir. Özellikle kapitalizm ile sosyalizm arasındaki mücadele, birçok ülkenin ekonomik yapısını belirlemiştir.", AppUserId = "c026e6a3-3553-43a7-83be-5be269d7e526", QuestionId = 7 , CreatedAt = DateTime.Now.AddDays(-1)},
    new Answer { AnswerText = "Siyasi ideolojilerin, bireylerin özgürlük anlayışları üzerinde büyük etkisi vardır. Sosyalizm, daha kolektif bir yaklaşım benimserken, kapitalizm bireysel hakları ve serbest piyasa ekonomisini vurgular.", AppUserId = "f34759fe-774b-454e-a5ab-822a263faad3", QuestionId = 8 ,CreatedAt = DateTime.Now.AddDays(-6) },
    new Answer { AnswerText = "Siyaset, halkın refahını artırmak için önemli bir araçtır. Ancak yanlış politikalar toplumsal eşitsizliklere ve sosyal huzursuzluklara yol açabilir.", AppUserId = "b1eb8555-e66c-4c65-8eed-abb0824d5796", QuestionId = 7,CreatedAt = DateTime.Now.AddDays(-3) },

    // **Bilim Sorusu ve Cevapları** (2 Cevap)
    new Answer { AnswerText = "Bilimsel keşifler, insan yaşamını iyileştirmede önemli bir rol oynuyor. Özellikle sağlık alanındaki gelişmeler, insanların daha uzun ve sağlıklı yaşamalarını sağladı.", AppUserId = "c026e6a3-3553-43a7-83be-5be269d7e526", QuestionId = 9 ,CreatedAt = DateTime.Now.AddDays(-2) },
    new Answer { AnswerText = "Bilimsel çalışmalar, toplumların refah seviyesini arttırmada çok önemli bir etkendir. Yenilikçi teknolojiler, enerji üretiminden sağlık sektörüne kadar birçok alanda devrim yaratıyor.", AppUserId = "f34759fe-774b-454e-a5ab-822a263faad3", QuestionId = 9 , CreatedAt = DateTime.Now.AddDays(-4)},

    // **Kültür Sorusu ve Cevapları** (3 Cevap)
    new Answer { AnswerText = "Kültürler arası etkileşim, insanların dünyaya bakış açısını genişletir. Bir toplumun kültürü, başka bir toplumla etkileşime girdiğinde, yeni düşünceler ve değerler ortaya çıkar.", AppUserId = "b1eb8555-e66c-4c65-8eed-abb0824d5796", QuestionId = 11,CreatedAt = DateTime.Now.AddDays(-5) },
    new Answer { AnswerText = "Kültürel değişim, toplumların gelişmesinde önemli bir rol oynar. Bir toplumun kültürel alışkanlıkları, o toplumun diğer toplumlarla olan ilişkilerini doğrudan etkiler.", AppUserId = "f34759fe-774b-454e-a5ab-822a263faad3", QuestionId = 13 , CreatedAt = DateTime.Now.AddDays(-6)},
    new Answer { AnswerText = "Kültürler arası etkileşim, daha geniş bir perspektiften bakmamızı sağlar. Her kültür, kendi değerlerini başka kültürlerle paylaşarak, insanlar arasında anlayış ve hoşgörü oluşturur.", AppUserId = "c026e6a3-3553-43a7-83be-5be269d7e526", QuestionId = 13 ,CreatedAt = DateTime.Now.AddDays(-11)},

    // **Spor Sorusu ve Cevapları** (2 Cevap)
    new Answer { AnswerText = "Sporun fiziksel ve zihinsel faydaları büyüktür. Düzenli spor yapmak, kalp sağlığını iyileştirir ve insanların daha enerjik olmalarını sağlar. Aynı zamanda stresi azaltır ve ruh halini iyileştirir.", AppUserId = "c026e6a3-3553-43a7-83be-5be269d7e526", QuestionId = 14 },
    new Answer { AnswerText = "Spor, sadece fiziksel sağlığı değil, aynı zamanda ruhsal sağlığı da iyileştirir. Düzenli egzersiz yapmak, depresyon gibi zihinsel sağlık sorunlarına karşı koruyucu bir etki yapar.", AppUserId = "f34759fe-774b-454e-a5ab-822a263faad3", QuestionId =14 ,CreatedAt = DateTime.Now.AddDays(-12)}
};

                foreach (var a in answers)
                {
                    Random random = new Random();
                    int UserNumber = random.Next(3, 6);

                    a.AppUserId = userList[UserNumber].Id;
                    await context.Answers.AddAsync(a);
                }

                await context.SaveChangesAsync();





            }

        }
    }
}
