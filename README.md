# SFTP Deployment Tool

Este é um utilitário simples para realizar deploys via SSH/SFTP. Ele suporta upload de arquivos/diretórios e execução de comandos remotos no servidor, você pode personalizar a ferramenta de acordo com suas necessidades.

## Funcionalidades

- Upload de diretórios locais para diretórios remotos via SFTP.
- Suporte a autenticação por senha ou chave privada.
- Execução de comandos no servidor via SSH.

## Configuração

1. **Instale as dependências**: Este projeto utiliza a biblioteca [SSH.NET](https://github.com/sshnet/SSH.NET). Certifique-se de adicioná-la ao seu projeto. Também, é possível adicionar o pacote Nuget RencySSH

2. **Configurações de servidor**:
   - Substitua as informações de host, usuário e autenticação no código conforme seu ambiente.
   - Use o caminho para sua chave privada no formato `.ppk` ou configure a senha diretamente.

3. **Personalize os caminhos**:
   - Atualize os caminhos dos diretórios locais e remotos no código:
     - `LocalApiPath` / `RemoteApiPath`
     - `LocalSitePath` / `RemoteSitePath`

## Uso

1. Compile o projeto.
2. Execute o programa. Ele irá:
   - Parar o serviço remoto configurado.
   - Fazer upload dos arquivos/diretórios.
   - Reiniciar o serviço.

