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

// 啟動伺服器，監聽 3000 端口
const start = async () => {
  try {
    await fastify.listen({ port: 3000, host: '0.0.0.0' });
    console.log('Server is running on http://0.0.0.0:3000');
  } catch (err) {
    fastify.log.error(err);
    process.exit(1);
  }
};

start();