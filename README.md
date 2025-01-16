# MTH404EPROJE



## Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [MySQL](https://www.mysql.com/downloads/)

### Adımlar

1. **Projeyi Klonlayın:**

    ```bash
    git clone https://github.com/kullaniciadi/nugetclone.git
    cd nugetclone
    ```

2. **.env Dosyasını Düzenleyin:**

    Proje dizininde bulunan `.env_example` dosyasını açın ve veritabanı bağlantı bilgilerini kendi bilgilerinize göre düzenleyin. Örneğin:

    ```properties
    DefaultConnection=server=;database=;user=;password=;port=3306;Pooling=false;
    ```

    - **server:** Veritabanı sunucusunun adresi (örneğin, `127.0.0.1`).
    - **database:** Veritabanı adı.
    - **user:** Veritabanı kullanıcı adı 
    - **password:** Veritabanı kullanıcı şifresi 
    - **port:** Veritabanı bağlantı noktası 

    Daha sonra .env_example adını .env yapın.

    C:\Users\meteb_xm3wyja\Desktop\efec#proje\nugetclone\API\Program.cs

    Daha sonra bu pathteki koda giderek 

    ```csharp
    Env.Load("C:/Users/meteb_xm3wyja/Desktop/efec#proje/.env");
    ```
    Değeri düzenlediğiniz .env dosyasının pathini yapıştırın.

    




3. **Yeni Migration Oluşturun ve Veritabanını Güncelleyin:**

    NugetCloneun(cd nugetclone) olduğu pathe giderek aşağıdaki komutları kullanarak yeni bir migration oluşturun ve veritabanını güncelleyin:

    ```bash
    dotnet ef migrations add InitialCreate -s API -p Persistence
    dotnet ef database update -s API -p Persistence
    ```

4. **Projeyi Çalıştırın:**

    Projeyi çalıştırmak için aşağıdaki komutu kullanın:

    API pathine cd ile gidin

    ```bash
    dotnet run    
    ```

