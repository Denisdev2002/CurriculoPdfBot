# 🤖 CVPdfBot - Gerador de Currículos via Telegram com .NET 8

Projeto criado com .NET 8 e a API do Telegram para construir currículos automaticamente, através de um fluxo de perguntas interativas enviadas via chatbot.

---

## 📦 Funcionalidades

- Bot do Telegram com Webhook via ASP.NET Core
- Fluxo inteligente de perguntas sobre o usuário
- Armazenamento em memória do estado da conversa (`ConversationState`)
- Geração de currículo em **3 estilos de template (Moderno, Clássico, Básico)**
- Visualização das opções de template por imagens
- Geração de currículo final em **PDF** usando Razor + DinkToPdf *(em andamento)*
- Encerramento automático de sessões inativas após 5 minutos

---

## 🛠️ Tecnologias Utilizadas

- .NET 8
- ASP.NET Core Web API
- RazorLight (template engine)
- DinkToPdf (geração de PDF a partir de HTML)
- Telegram.Bot SDK
- Hosted Services (SessionCleanerService para controle de inatividade)
- Ngrok (em modo dev para webhook do Telegram)

---

## 🧪 Como testar localmente

1. Clone o repositório
2. Configure seu token do bot no `appsettings.Development.json`:

```json
{
  "TelegramBotToken": "SEU_TOKEN_DO_TELEGRAM"
}
````

3. Rode o projeto:
dotnet run

4. Inicie o Ngrok (caso esteja em localhost):
ngrok http 5020

![Captura de Tela do Doxygen](images/doxygen_screenshot.png)
_Este é o resultado gerado pelo Doxygen após rodar a documentação._

6. Registre o webhook:
https://api.telegram.org/bot<SEU_TOKEN>/setWebhook?url=https://<NGROK_URL>/api/telegram

7. Inicie a conversa com o bot enviando /start
   
---

## 📸 Templates

Os templates são exibidos via imagem antes da seleção final. Os arquivos .cshtml são usados como base e renderizados dinamicamente. Após seleção, o currículo é gerado com base no template escolhido.

---

## 📄 Documentação do Código

A documentação do código é gerada automaticamente utilizando Doxygen. Para visualizar a documentação localmente, siga os passos abaixo:

Passos para gerar a documentação:
Clone o repositório.

Instale o Doxygen se ainda não tiver:

No Windows, você pode instalar através do instalador.

No Linux, pode instalar via apt-get ou brew no macOS.

No diretório do projeto, execute o Doxygen com o arquivo de configuração Doxyfile:

Após a execução, a documentação estará disponível no diretório C:/doxygen-curriculopdfbot (ou conforme configurado no Doxyfile).

Abra o arquivo index.html no seu navegador para visualizar a documentação gerada.

## 🤝 Contribuindo

Sinta-se à vontade para abrir issues e enviar pull requests!

##  🤝 Contribuindo

Sinta-se à vontade para abrir issues e enviar pull requests!

---

## 📃 Licença

Este projeto está licenciado sob a licença MIT.
