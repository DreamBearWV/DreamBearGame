const Fastify = require('fastify');
const fastify = Fastify({ logger: true });

// 允許跨域請求
fastify.register(require('@fastify/cors'), { 
  origin: '*' 
});

// 根路由測試
fastify.get('/', async (request, reply) => {
  return { status: 'ok', message: 'Hello World from Raspberry Pi 5!' };
});

// 健康檢查 API 路由
fastify.get('/api/health', async (request, reply) => {
  return { status: 'ok', message: '樹莓派 Node.js 後端運作正常！', timestamp: new Date() };
});

// 新增分數/存檔 POST API
fastify.post('/api/score', async (request, reply) => {
  // 從 request.body 讀取前端傳來的 JSON 資料
  const { username, score } = request.body || {};

  // 基本驗證
  if (!username || score === undefined) {
    return reply.status(400).send({
      status: 'error',
      message: '缺少 username 或 score 參數'
    });
  }

  // 模擬存檔邏輯 (未來可在此寫入 SQLite / MongoDB)
  console.log(`[分數存檔] 玩家: ${username} | 分數: ${score}`);

  return {
    status: 'success',
    message: '分數成功儲存！',
    data: {
      username,
      score,
      timestamp: new Date().toISOString()
    }
  };
});

// 啟動伺服器，監聽 3000 端口
const start = async () => {
  try {
    await fastify.listen({ port: 3001, host: '0.0.0.0' });
    console.log('Server is running on http://0.0.0.0:3000');
  } catch (err) {
    fastify.log.error(err);
    process.exit(1);
  }
};

start();