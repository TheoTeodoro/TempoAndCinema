CREATE TABLE IF NOT EXISTS Filmes (
      Id INTEGER PRIMARY KEY AUTOINCREMENT,
      TmdbId INTEGER,
      Titulo TEXT NOT NULL,
      TituloOriginal TEXT,
      Sinopse TEXT,
      DataLancamento TEXT,
      Genero TEXT,
      PosterPath TEXT,
      Lingua TEXT,
      Duracao INTEGER,
      NotaMedia REAL,
      ElencoPrincipal TEXT,
      CidadeReferencia TEXT,
      Latitude REAL,
      Longitude REAL,
      DataCriacao TEXT,
      DataAtualizacao TEXT
);
