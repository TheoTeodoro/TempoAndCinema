# 🎬 *Tempo & Cinema — Seu Catálogo Inteligente de Filmes*

Bem-vindo ao **Tempo & Cinema**, uma aplicação web criada para transformar a forma como você descobre, acompanha e gerencia filmes.
Ela combina **cinema + clima + geolocalização** para entregar uma experiência única: seu catálogo pessoal enriquecido com dados da TMDB e informações de **temperatura máxima, mínima e clima da cidade associada ao filme** no momento da consulta.

---

## 👨‍💻 **Autores**
* **Alicy Rodrigues da Costa**
* **Lucayan Felipe Teixeira da Silva**
* **Théo Teodoro Novais**
---
## ⭐ **O que é o Tempo & Cinema?**

Uma aplicação **ASP.NET MVC** que integra recursos poderosos:

* ✔ Catálogo de filmes dividido em categorias
* ✔ Consumo da API **TMDB** para buscas e detalhes completos
* ✔ Repositório local persistente (**SQLite**)
* ✔ Geolocalização (cidade, latitude, longitude)
* ✔ Dados meteorológicos integrados (API **Open-Meteo**)
* ✔ Exportação de filmes para **CSV**
* ✔ Edição completa dos filmes salvos
* ✔ Experiência organizada, bonita e fácil de usar

Tudo isso pensado para criar **o catálogo de filmes mais completo que você já viu**.

---

## 🎞️ **Recursos Principais**

---

### 🔍 **Busca inteligente**

Na barra de pesquisa, você pode:

* Buscar qualquer filme pelo nome
* Aplicar filtros como gênero
* Receber vários resultados correspondentes
* Navegar entre páginas de retorno
* Visualizar lista completa de filmes relevantes

A busca é poderosa porque utiliza a **API TMDB**, atualizada diariamente.

---

### 📄 **Detalhes completos do filme**

Ao clicar em **"Ver detalhes"**, você tem acesso a um painel completo contendo:

* Título e título original
* Poster em alta resolução
* Data de lançamento
* Sinopse
* Gênero
* Idioma original
* Duração
* Nota média
* Trailers oficiais
* Imagens adicionais
* Elenco principal

Um verdadeiro **dossiê cinematográfico** ao alcance de um clique.

---

### 📌 **Salvar no catálogo local (persistência real)**

Gostou do filme? Basta clicar em:

👉 **"Salvar no banco de dados local"**

Ao salvar:

* O filme passa a fazer parte do seu catálogo privado
* Você define a cidade, latitude e longitude associadas ao filme
* Tudo é armazenado no banco **SQLite**
* Os detalhes ficam acessíveis mesmo **offline**
* É possível editar tudo posteriormente

---

### 🌦️ **Clima e filmes — uma experiência única**

Sempre que você visualizar um filme salvo, a aplicação mostra:

* Data do clima retornado pela API
* Temperatura máxima
* Temperatura mínima

Tudo com base na cidade/latitude/longitude que você definiu.

Isso permite experiências como:

> “Como estava o clima na cidade associada ao filme no dia da consulta?”

---

### 🗂️ **Gerenciamento completo dos filmes locais**

Na seção **Filmes Locais**, você poderá:

* ✔ Ver detalhes completos
* ✔ Editar qualquer campo
* ✔ Exportar para CSV com um clique
* ✔ Excluir permanentemente
* ✔ Consultar o clima associado

Tudo rápido, tudo integrado.

---

### 📤 **Exportação CSV premium**

Com apenas um botão, você exporta o filme com informações limpas e formatadas, seguindo:

* **RFC-4180**
* **Acentuação preservada com BOM UTF-8**
* **Aspas tratadas corretamente**
* **Sinopse higienizada**
* **Elenco unificado e organizado**

Perfeito para:

* Excel
* Google Sheets
* Ferramentas de análise
* Arquivamento pessoal

---

## 🧭 **Como funciona a aplicação ao abrir?**

Assim que você inicia o Tempo & Cinema, você vê um catálogo dividido em:

* **Em alta na semana**
* **Em cartaz**
* **Filmes salvos no repositório local**

(Categorias dinâmicas e organizadas.)

A experiência é **fluida, bonita e convidativa**.

Com poucos cliques, você encontra um filme, explora seus detalhes e salva no catálogo.

O objetivo é entregar a sensação de:

> “Um aplicativo digno de plataformas profissionais, mas totalmente personalizado para você.”

---

## 🚀 **Uma aplicação feita para impressionar**

O Tempo & Cinema foi desenvolvido com foco em:

* Boas práticas
* Arquitetura limpa
* Serviços e repositórios
* Controllers organizados
* Integrações reais com APIs externas
* Persistência robusta
* Exportação avançada
* Tratamento de erros e logs

Tudo pensado para entregar um produto de qualidade para nossos clientes!
