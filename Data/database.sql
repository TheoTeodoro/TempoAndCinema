CREATE TABLE Filmes (
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

    -- 🆕 NOVAS COLUNAS
                                      TrailerUrl TEXT,                -- URL do trailer oficial
                                      ElencoPrincipal TEXT,           -- Lista pequena JSON (até 10)
                                      BackdropsJson TEXT,             -- Array JSON das imagens principais (5–10)

                                      CidadeReferencia TEXT,
                                      Latitude REAL,
                                      Longitude REAL,
                                      DataCriacao TEXT,
                                      DataAtualizacao TEXT
);
