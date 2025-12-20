-- Database: SiparisTakip_OdevDB

CREATE TABLE Kullanicilar
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciAdi NVARCHAR(200) NOT NULL,
    Sifre NVARCHAR(200) NOT NULL,
    Rol NVARCHAR(50) NOT NULL
);

CREATE TABLE Urunler
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UrunAdi NVARCHAR(200) NOT NULL,
    Fiyat DECIMAL(18,2) NOT NULL,
    Stok INT NOT NULL
);

CREATE TABLE Siparisler
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciId INT NOT NULL FOREIGN KEY REFERENCES Kullanicilar(Id),
    Tarih DATETIME NOT NULL,
    Durum NVARCHAR(100) NOT NULL
);

CREATE TABLE SiparisDetaylar
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SiparisId INT NOT NULL FOREIGN KEY REFERENCES Siparisler(Id),
    UrunId INT NOT NULL FOREIGN KEY REFERENCES Urunler(Id),
    Adet INT NOT NULL,
    BirimFiyat DECIMAL(18,2) NOT NULL
);

-- Insert a default admin
INSERT INTO Kullanicilar (KullaniciAdi,Sifre,Rol) VALUES ('admin','admin','Admin');
