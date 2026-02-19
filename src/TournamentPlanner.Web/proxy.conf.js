module.exports = {
  '/api/**': {
    target:
      process.env['services__tournamentplanner-api__https__0'] ||
      process.env['services__tournamentplanner-api__http__0'] ||
      'https://localhost:7192',
    secure: false,
    changeOrigin: true,
    rewrite: (path) => path.replace(/^\/api/, ''),
  },
};
