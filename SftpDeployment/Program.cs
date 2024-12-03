using Renci.SshNet;
using System;
using System.IO;

namespace SftpDeployment
{
    public static class Sftp
    {
        // Diretórios locais e remotos - personalize conforme sua aplicação
        private const string LocalApiPath = @"C:\path\to\api\publish\";
        private const string RemoteApiPath = "/var/api/your.api.path/";
        private const string LocalSitePath = @"C:\path\to\site\dist\";
        private const string RemoteSitePath = "/var/www/your.website.path/html/";

        public static void Main(string[] args)
        {
            // Parar o serviço antes do deploy
            ExecuteCommandOnServer("sudo service your.api.service stop");

            // Limpar arquivos antigos (exemplo: CSS/JS chuncks do front-end)
            ExecuteCommandOnServer("rm -R /var/www/your.website.path/html/js");
            ExecuteCommandOnServer("rm -R /var/www/your.website.path/html/css");

            // Fazer upload de arquivos
            UploadDirectories(LocalApiPath, RemoteApiPath);
            UploadDirectories(LocalSitePath, RemoteSitePath);

            // Reiniciar o serviço após o deploy
            ExecuteCommandOnServer("sudo service your.api.service start");
        }

        public static void UploadDirectories(string localDirectoryPath, string remoteDirectoryPath)
        {
            // Substitua pelos seus detalhes do servidor
            string host = "your.server.host";
            int port = 22; // Porta padrão SSH
            string username = "your_username";
            string password = null; // Deixe null se for usar chave privada
            string privateKeyFilePath = @"C:\path\to\your\privateKey.ppk"; // Insira o caminho da sua chave privada

            var authenticationMethods = new List<AuthenticationMethod>();

            if (!string.IsNullOrEmpty(password))
                authenticationMethods.Add(new PasswordAuthenticationMethod(username, password));

            if (!string.IsNullOrEmpty(privateKeyFilePath) && File.Exists(privateKeyFilePath))
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(privateKeyFilePath)));

            var connectionInfo = new ConnectionInfo(host, port, username, authenticationMethods.ToArray());

            using (var sftp = new SftpClient(connectionInfo))
            {
                try
                {
                    sftp.Connect();
                    Console.WriteLine("Conexão SFTP bem-sucedida.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao conectar ao servidor SFTP: " + ex.Message);
                    return;
                }

                // Enviar o diretório local para o remoto
                UploadDirectory(sftp, localDirectoryPath, remoteDirectoryPath);

                sftp.Disconnect();
            }
        }

        private static void UploadDirectory(SftpClient sftp, string localDirectoryPath, string remoteDirectoryPath)
        {
            // Criar o diretório remoto se ele não existir
            if (!sftp.Exists(remoteDirectoryPath))
                sftp.CreateDirectory(remoteDirectoryPath);

            // Obter arquivos e subdiretórios locais
            var entries = Directory.GetFileSystemEntries(localDirectoryPath);

            foreach (var entry in entries)
            {
                if (Directory.Exists(entry))
                {
                    // Enviar diretório de forma recursiva
                    var remoteSubdirectoryPath = remoteDirectoryPath + "/" + Path.GetFileName(entry);
                    UploadDirectory(sftp, entry, remoteSubdirectoryPath);
                }
                else
                {
                    // Enviar arquivo
                    using (var fileStream = new FileStream(entry, FileMode.Open))
                    {
                        var remoteFilePath = remoteDirectoryPath + "/" + Path.GetFileName(entry);
                        Console.WriteLine($"Enviando arquivo: {entry} para: {remoteFilePath}");
                        sftp.UploadFile(fileStream, remoteFilePath);
                    }
                }
            }
        }

        public static void ExecuteCommandOnServer(string command)
        {
            // Substitua pelos seus detalhes do servidor
            string host = "your.server.host";
            string username = "your_username";
            string password = "your_password"; // Deixe null se for usar chave privada
            string privateKeyFilePath = @"C:\path\to\your\privateKey.ppk";

            var authenticationMethods = new List<AuthenticationMethod>();

            if (!string.IsNullOrEmpty(password))
                authenticationMethods.Add(new PasswordAuthenticationMethod(username, password));

            if (!string.IsNullOrEmpty(privateKeyFilePath) && File.Exists(privateKeyFilePath))
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(privateKeyFilePath)));

            var connectionInfo = new ConnectionInfo(host, 22, username, authenticationMethods.ToArray());

            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    client.Connect();
                    Console.WriteLine("Conexão SSH bem-sucedida.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao conectar ao servidor SSH: " + ex.Message);
                    return;
                }

                // Executar comando remoto
                var result = client.RunCommand(command);
                Console.WriteLine(result.Result);

                client.Disconnect();
            }
        }
    }
}
