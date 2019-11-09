module.exports = {
  entry: __dirname+ '\\src\\index.js',
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loader: 'babel-loader'
      }
    ]
  },
  output: {
    filename: 'thebundle.js',
    path: __dirname + '\\static\\js'
  }
};
