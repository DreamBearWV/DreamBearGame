const Fastify = require('fastify');
const fastify = Fastify({ logger: true });
const Database = require('better-sqlite3');
const path = require('path');

// ==========================================
// 1. 初始化 SQLite 資料庫與 WAL 效能模式
// ==========================================
const db = new Database(path.join(__dirname, 'game.db'));

// 【專業優化】：開啟 WAL 模式與 NORMAL 同步，大幅提升併發效能並保護樹莓派 SD 卡壽命
db.pragma('journal_mode = WAL');
db.pragma('synchronous = NORMAL');

// 自動建立玩家分數資料表 (若不存在)
db.exec(`
  CREATE TABLE IF NOT EXISTS scores (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL,
    score INTEGER NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
  )
`);

// ==========================================
// 2. 跨域保護與中間件 (CORS)
// ==========================================
fastify.register(require('@fastify/cors'), { 
  origin: '*' 
});

// ==========================================
// 3. API 路由定義
// ==========================================

// 根路由測試 (保留原程式碼)
fastify.get('/', async (request, reply) => {
  return { status: 'ok', message: 'Hello World from Raspberry Pi 5!' };
});

// 健康檢查 API 路由 (保留原程式碼 + timestamp)
fastify.get('/api/health', async (request, reply) => {
  return { 
    status: 'ok', 
    message: '樹莓派 Node.js 後端運作正常！', 
    timestamp: new Date() 
  };
});

// 分數/存檔 POST API (正式升級為寫入 SQLite 資料庫)
fastify.post('/api/score', async (request, reply) => {
  // 從 request.body 讀取前端傳來的 JSON 資料
  const { username, score } = request.body || {};

  // 基本驗證 (保留原邏輯)
  if (!username || score === undefined) {
    return reply.status(400).send({
      status: 'error',
      message: '缺少 username 或 score 參數'
    });
  }

  // 使用 PreparedStatement 防止 SQL 注入，寫入 SQLite
  const stmt = db.prepare('INSERT INTO scores (username, score) VALUES (?, ?)');
  const result = stmt.run(username, score);

  console.log(`[分數存檔] 玩家: ${username} | 分數: ${score} | DB ID: ${result.lastInsertRowid}`);

  return {
    status: 'success',
    message: '分數成功儲存至 SQLite！',
    data: {
      id: result.lastInsertRowid,
      username,
      score,
      timestamp: new Date().toISOString()
    }
  };
});

// 排行榜 Top 10 GET API (新增：供前端展示高分榜)
fastify.get('/api/scores/top', async (request, reply) => {
  const stmt = db.prepare('SELECT username, score, created_at FROM scores ORDER BY score DESC LIMIT 10');
  const topScores = stmt.all();

  return {
    status: 'success',
    data: topScores
  };
});

// ==========================================
// 4. Graceful Shutdown (優雅關閉)
// 確保 PM2 重啟或 GitHub Actions 部署時資料庫不損壞
// ==========================================
const cleanup = async () => {
  console.log('\n[System] 收到關閉訊號，正準備優雅關閉 Fastify 與 SQLite...');
  try {
    await fastify.close();
    if (db && db.open) {
      db.close();
      console.log('[System] SQLite 資料庫連線已安全關閉。');
    }
  } catch (err) {
    console.error('[System] 關閉過程發生錯誤:', err);
  } finally {
    process.exit(0);
  }
};

process.on('SIGINT', cleanup);
process.on('SIGTERM', cleanup);

// ==========================================
// 5. 啟動伺服器，監聽 3001 端口
// ==========================================
const start = async () => {
  try {
    await fastify.listen({ port: 3001, host: '0.0.0.0' });
    console.log('Server is running on http://0.0.0.0:3001');
  } catch (err) {
    fastify.log.error(err);
    process.exit(1);
  }
};

start();