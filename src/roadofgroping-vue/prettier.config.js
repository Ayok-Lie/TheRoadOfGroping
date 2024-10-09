module.exports = {
  printWidth: 100, // 每行最多100个字符
  tabWidth: 2, // 缩进2个字符
  useTabs: false, // 不使用tab缩进
  semi: true, // 代码结尾分号
  singleQuote: true, // 使用单引号
  quoteProps: 'as-needed', // 仅在需要时在对象属性周围添加引号
  jsxSingleQuote: false, // jsx中使用双引号
  trailingComma: 'none', // 多行逗号分隔语法中不打印尾随逗号
  bracketSpacing: true, // 对象定义花括号的空格
  bracketSameLine: false, // 多行html元素的>另起一行
  arrowParens: 'always', // 箭头函数参数始终使用括号
  rangeStart: 0, // 格式化起点，第0行
  rangeEnd: Infinity, // 格式化起点，最后一行
  proseWrap: 'preserve', // 处理文本换行
  htmlWhitespaceSensitivity: 'strict', // 所有tag缺少空格都需要严格校验
  vueIndentScriptAndStyle: true, // 缩进vue文件中的 script、style 标签
  endOfLine: 'crlf', // 文件结束符使用CRLF
  embeddedLanguageFormatting: 'off' // 不格式化嵌入在文件中带引号的代码
};
