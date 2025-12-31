const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
const PORT = 3000;

app.use(cors());
app.use(bodyParser.json());
app.use(express.static('public'));

// baza danych
const db = new sqlite3.Database('./movies.db', (err) => {
    if (err) console.error(err.message);
    else console.log('Baza filmów podłączona.');
});

db.serialize(() => {
    // tabelka movies
    db.run(`CREATE TABLE IF NOT EXISTS movies (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT,
        year INTEGER
    )`);

    // tabelka ratings z walidacja
    db.run(`CREATE TABLE IF NOT EXISTS ratings (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        movie_id INTEGER,
        score INTEGER CHECK(score >= 1 AND score <= 5),
        FOREIGN KEY(movie_id) REFERENCES movies(id)
    )`);

    // dane startowe
    db.get("SELECT count(*) as count FROM movies", (err, row) => {
        if (row.count === 0) {
            console.log("Dodaję filmy startowe...");
            const stmt = db.prepare("INSERT INTO movies (title, year) VALUES (?, ?)");
            stmt.run("Inception", 2010);
            stmt.run("Matrix", 1999);
            stmt.run("Arrival", 2016);
            stmt.finalize();

            // dodawanie ocen
            setTimeout(() => {
                const r = db.prepare("INSERT INTO ratings (movie_id, score) VALUES (?, ?)");
                r.run(1, 5); r.run(1, 4); 
                r.run(2, 5);             
                r.run(3, 4); r.run(3, 5); 
                r.finalize();
            }, 1000);
        }
    });
});

// api
app.get('/api/movies', (req, res) => {
    let sql = `
        SELECT 
            m.id, m.title, m.year,
            COUNT(r.id) as votes,
            COALESCE(ROUND(AVG(r.score), 2), 0) as avg_score
        FROM movies m
        LEFT JOIN ratings r ON m.id = r.movie_id
    `;

    const params = [];
    if (req.query.year) {
        sql += " WHERE m.year = ?";
        params.push(req.query.year);
    }

    sql += " GROUP BY m.id ORDER BY avg_score DESC";

    if (req.query.limit) {
        sql += " LIMIT ?";
        params.push(req.query.limit);
    }

    db.all(sql, params, (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows);
    });
});

app.post('/api/movies', (req, res) => {
    const { title, year } = req.body;
    if (!title || !year) return res.status(400).json({ error: "Brak danych" });

    db.run("INSERT INTO movies (title, year) VALUES (?, ?)", [title, year], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ id: this.lastID });
    });
});

app.post('/api/ratings', (req, res) => {
    const { movie_id, score } = req.body;

    if (score < 1 || score > 5) {
        return res.status(400).json({ error: "Ocena musi być z zakresu 1-5" });
    }

    db.run("INSERT INTO ratings (movie_id, score) VALUES (?, ?)", [movie_id, score], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ id: this.lastID, message: "Ocena dodana" });
    });
});

app.listen(PORT, () => console.log(`Kino działa na http://localhost:${PORT}`));