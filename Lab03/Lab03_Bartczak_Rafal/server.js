const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
const PORT = 3000;

app.use(cors()); 
app.use(bodyParser.json()); 
app.use(express.static('public')); 

// konfiguracja bazy danych
const db = new sqlite3.Database('./blog.db', (err) => {
    if (err) console.error(err.message);
    else console.log('Połączono z bazą SQLite.');
});

// tworzenie tabel i danych
db.serialize(() => {
    // Tabela Posty
    db.run(`CREATE TABLE IF NOT EXISTS posts (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT,
        body TEXT,
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    )`);

    db.run(`CREATE TABLE IF NOT EXISTS comments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        post_id INTEGER,
        author TEXT,
        body TEXT,
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        approved INTEGER DEFAULT 0, -- 0 = ukryty, 1 = widoczny
        FOREIGN KEY(post_id) REFERENCES posts(id)
    )`);

    db.get("SELECT count(*) as count FROM posts", (err, row) => {
        if (row.count === 0) {
            console.log("Dodaję dane startowe...");
            
            const stmt = db.prepare("INSERT INTO posts (title, body) VALUES (?, ?)");
            stmt.run("Witamy na blogu", "To jest pierwszy post testowy w Node.js");
            stmt.finalize();

            setTimeout(() => {
                db.run("INSERT INTO comments (post_id, author, body, approved) VALUES (1, 'Admin', 'Komentarz widoczny', 1)");
                db.run("INSERT INTO comments (post_id, author, body, approved) VALUES (1, 'Spamer', 'Komentarz do moderacji', 0)");
            }, 1000);
        }
    });
});

// endpointy api
app.get('/api/posts', (req, res) => {
    db.all("SELECT * FROM posts ORDER BY created_at DESC", [], (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows);
    });
});

app.post('/api/posts', (req, res) => {
    const { title, body } = req.body;
    db.run("INSERT INTO posts (title, body) VALUES (?, ?)", [title, body], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ id: this.lastID, title, body });
    });
});

app.get('/api/posts/:id', (req, res) => {
    db.get("SELECT * FROM posts WHERE id = ?", [req.params.id], (err, row) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(row);
    });
});

app.get('/api/posts/:id/comments', (req, res) => {
    const sql = "SELECT * FROM comments WHERE post_id = ? AND approved = 1 ORDER BY created_at DESC";
    db.all(sql, [req.params.id], (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows);
    });
});

app.post('/api/posts/:id/comments', (req, res) => {
    const { author, body } = req.body;
    const postId = req.params.id;
    
    const sql = "INSERT INTO comments (post_id, author, body, approved) VALUES (?, ?, ?, 0)";
    
    db.run(sql, [postId, author, body], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ id: this.lastID, approved: 0, message: "Czeka na moderację" });
    });
});

app.get('/api/moderation/pending', (req, res) => {
    const sql = `
        SELECT comments.*, posts.title as post_title 
        FROM comments 
        JOIN posts ON comments.post_id = posts.id
        WHERE approved = 0`;
        
    db.all(sql, [], (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows);
    });
});

app.post('/api/comments/:id/approve', (req, res) => {
    const sql = "UPDATE comments SET approved = 1 WHERE id = ?";
    db.run(sql, [req.params.id], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ message: "Zatwierdzono", changes: this.changes });
    });
});

// start serwera
app.listen(PORT, () => {
    console.log(`Serwer działa na http://localhost:${PORT}`);
});