VIDEO: 


https://github.com/user-attachments/assets/e899c443-bb9c-4509-92d6-f17eb99a6a52



DapperKaggle – Futbol Analitik Dashboardu (ASP.NET Core + Dapper)
Kaggle’daki büyük futbol veri setiyle (1M+ satır) çalışan, ASP.NET Core MVC + Dapper tabanlı mini analitik uygulama.
Modern Tailwind arayüz, Chart.js grafikler, kulüp/oyuncu sayfaları ve saha üzerinde sürükle-bırak ilk 11 ile gelir.

✨ Özellikler
Dashboard

KPI’lar: toplam oyuncu, kulüp sayısı, toplam piyasa değeri, ort. yaş

Grafikler: En Değerli Oyuncular, Milli Oyuncu – Top 10 Kulüp, Pozisyona Göre Dağılım (donut), Yaş Dağılımı

Lig seçimi (TR1/GB1/ES1) + opsiyonel kulüp seçimi

Kulüpler (Clubs)
Lig filtreli liste (TR1/GB1/ES1), arama, sayfalama, “Kadro” ve “Maçlar” kısayolları

Oyuncular (Players)
Sol listede tüm kadro, sağda modern saha (4-2-3-1 / 4-3-3 / 4-4-2), drag&drop ve localStorage ile kalıcı 11; oyuncu detay kartı

Maçlar (Matches)
Kart görünümü: skor, W/D/L rozetleri, saha (home/away/neutral), teknik direktörler; filtreler + sayfalama

🧰 Teknoloji
ASP.NET Core MVC, Dapper

Tailwind CSS, Chart.js

SQL Server (veri depolama)

🧠 Mimari
Service/Repository (Dapper)

ClubsManager – kulüp listesi/sayım

ClubGamesManager – maç listesi/sayım

AnalyticsManager – tek DTO’lu Dashboard (2 basit SELECT + LINQ ile tüm metrikler/grafikler)

Tek Dashboard DTO: DashboardDto
İçerir: top_players, club_nat_players_top, position_counts, age_buckets, clubs, club_top_players ve özet metrikler.

🧩 Ekranlar
Dashboard: KPI + 4 grafik; kulüp seçilirse sağ panelde en değerli 5 oyuncu

Clubs: lig sekmeleri, arama, page size; her satırdan Kadro/Maçlar kısayolu

Players: yüzde koordinatlı modern saha, formasyon seçici, sürükle-bırak, oyuncu detay kartı

Matches: kart görünümü; skor mini barı, rozetler, teknik direktörler, filtreler
