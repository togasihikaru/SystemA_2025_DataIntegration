const fs = require('fs');
const path = require('path');

// より高度なMarkdownパーサー（テーブル対応）
function parseMarkdown(markdown) {
  let html = '';
  const lines = markdown.split('\n');
  let inTable = false;
  let tableRows = [];
  let inList = false;
  
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    const trimmedLine = line.trim();
    
    // テーブルの検出
    if (trimmedLine.startsWith('|') && trimmedLine.endsWith('|')) {
      if (!inTable) {
        inTable = true;
        tableRows = [];
      }
      // ヘッダー区切り線をスキップ
      if (trimmedLine.match(/^[\|\s\-:]+\|$/)) {
        continue;
      }
      const cells = trimmedLine.split('|').slice(1, -1).map(cell => cell.trim());
      tableRows.push(cells);
      continue;
    } else {
      if (inTable) {
        html += '<table border="1" cellpadding="5" cellspacing="0" style="border-collapse: collapse; width: 100%; margin: 15px 0;">\n';
        if (tableRows.length > 0) {
          // 最初の行をヘッダーとして扱う
          html += '<thead><tr>';
          tableRows[0].forEach(cell => {
            html += `<th style="background-color: #f2f2f2; font-weight: bold;">${formatText(cell)}</th>`;
          });
          html += '</tr></thead>\n<tbody>\n';
          
          // 残りの行をデータ行として扱う
          for (let j = 1; j < tableRows.length; j++) {
            html += '<tr>';
            tableRows[j].forEach(cell => {
              html += `<td>${formatText(cell)}</td>`;
            });
            html += '</tr>\n';
          }
        }
        html += '</tbody></table>\n';
        inTable = false;
        tableRows = [];
      }
    }
    
    // リストの処理
    if (trimmedLine.startsWith('- ')) {
      if (!inList) {
        html += '<ul>\n';
        inList = true;
      }
      html += `<li>${formatText(trimmedLine.substring(2))}</li>\n`;
      continue;
    } else {
      if (inList) {
        html += '</ul>\n';
        inList = false;
      }
    }
    
    // ヘッダー
    if (trimmedLine.startsWith('#### ')) {
      html += `<h4>${formatText(trimmedLine.substring(5))}</h4>\n`;
    } else if (trimmedLine.startsWith('### ')) {
      html += `<h3>${formatText(trimmedLine.substring(4))}</h3>\n`;
    } else if (trimmedLine.startsWith('## ')) {
      html += `<h2>${formatText(trimmedLine.substring(3))}</h2>\n`;
    } else if (trimmedLine.startsWith('# ')) {
      html += `<h1>${formatText(trimmedLine.substring(2))}</h1>\n`;
    }
    // 空行
    else if (trimmedLine === '') {
      html += '<p></p>\n';
    }
    // 通常のテキスト（テーブルやリストでない場合）
    else if (!trimmedLine.startsWith('|') && !trimmedLine.startsWith('-')) {
      const formatted = formatText(trimmedLine);
      if (formatted.trim()) {
        html += `<p>${formatted}</p>\n`;
      }
    }
  }
  
  // 最後のテーブルを処理
  if (inTable && tableRows.length > 0) {
    html += '<table border="1" cellpadding="5" cellspacing="0" style="border-collapse: collapse; width: 100%; margin: 15px 0;">\n';
    html += '<thead><tr>';
    tableRows[0].forEach(cell => {
      html += `<th style="background-color: #f2f2f2; font-weight: bold;">${formatText(cell)}</th>`;
    });
    html += '</tr></thead>\n<tbody>\n';
    
    for (let j = 1; j < tableRows.length; j++) {
      html += '<tr>';
      tableRows[j].forEach(cell => {
        html += `<td>${formatText(cell)}</td>`;
      });
      html += '</tr>\n';
    }
    html += '</tbody></table>\n';
  }
  
  // 最後のリストを処理
  if (inList) {
    html += '</ul>\n';
  }
  
  return html;
}

// テキストフォーマット（太字、改行など）
function formatText(text) {
  if (!text) return '';
  // 太字を変換
  text = text.replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>');
  // <br>タグを変換
  text = text.replace(/<br>/g, '<br>');
  return text;
}

// 完全なHTMLドキュメントを作成
function createHtmlDocument(content, title) {
  return `<!DOCTYPE html>
<html lang="ja">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>${title}</title>
  <style>
    @page {
      size: A4;
      margin: 20mm;
    }
    body {
      font-family: 'MS Gothic', 'Yu Gothic', 'Meiryo', 'MS PGothic', sans-serif;
      line-height: 1.6;
      color: #333;
      background: white;
      margin: 0;
      padding: 0;
    }
    h1 {
      border-bottom: 2px solid #333;
      padding-bottom: 10px;
      margin-top: 30px;
      margin-bottom: 20px;
      font-size: 24px;
    }
    h2 {
      border-bottom: 1px solid #666;
      padding-bottom: 5px;
      margin-top: 25px;
      margin-bottom: 15px;
      font-size: 20px;
    }
    h3 {
      margin-top: 20px;
      margin-bottom: 10px;
      color: #444;
      font-size: 16px;
    }
    h4 {
      margin-top: 15px;
      margin-bottom: 8px;
      color: #555;
      font-size: 14px;
    }
    p {
      margin: 8px 0;
    }
    table {
      border-collapse: collapse;
      width: 100%;
      margin: 15px 0;
      font-size: 0.9em;
      page-break-inside: avoid;
    }
    table th, table td {
      border: 1px solid #ddd;
      padding: 8px;
      text-align: left;
      vertical-align: top;
    }
    table th {
      background-color: #f2f2f2;
      font-weight: bold;
    }
    ul {
      margin: 10px 0;
      padding-left: 30px;
    }
    li {
      margin: 5px 0;
    }
    @media print {
      h1, h2, h3, h4 {
        page-break-after: avoid;
      }
      table {
        page-break-inside: avoid;
      }
    }
  </style>
</head>
<body>
${content}
</body>
</html>`;
}

// メイン処理
async function main() {
  const inputFile = path.join(__dirname, 'テスト', '内部結合テスト仕様書.md');
  const outputHtmlFile = path.join(__dirname, 'テスト', '内部結合テスト仕様書.html');
  const outputPdfFile = path.join(__dirname, 'テスト', '内部結合テスト仕様書.pdf');

  try {
    console.log('Markdownファイルを読み込んでいます...');
    const markdown = fs.readFileSync(inputFile, 'utf-8');
    
    console.log('HTMLに変換しています...');
    const htmlContent = parseMarkdown(markdown);
    const fullHtml = createHtmlDocument(htmlContent, '内部結合テスト仕様書');
    
    console.log('HTMLファイルを作成しています...');
    fs.writeFileSync(outputHtmlFile, fullHtml, 'utf-8');
    console.log(`✓ HTMLファイルを作成しました: ${outputHtmlFile}`);
    
    // Puppeteerを使用してPDFに変換
    try {
      console.log('Puppeteerを読み込んでいます...');
      const puppeteer = require('puppeteer');
      
      console.log('ブラウザを起動しています...');
      const browser = await puppeteer.launch({
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
      });
      
      const page = await browser.newPage();
      await page.setContent(fullHtml, { waitUntil: 'networkidle0' });
      
      console.log('PDFを生成しています...');
      await page.pdf({
        path: outputPdfFile,
        format: 'A4',
        margin: {
          top: '20mm',
          right: '20mm',
          bottom: '20mm',
          left: '20mm'
        },
        printBackground: true
      });
      
      await browser.close();
      console.log(`✓ PDFファイルを作成しました: ${outputPdfFile}`);
    } catch (puppeteerError) {
      console.log('Puppeteerが利用できないため、HTMLファイルのみ作成しました。');
      console.log('PDFに変換するには、以下のいずれかの方法を使用してください:');
      console.log('1. ブラウザでHTMLファイルを開き、「印刷」→「PDFとして保存」');
      console.log('2. npm install puppeteer を実行してから、このスクリプトを再実行');
      console.log(`HTMLファイル: ${outputHtmlFile}`);
    }
  } catch (error) {
    console.error('エラーが発生しました:', error.message);
    console.error(error.stack);
    process.exit(1);
  }
}

main();
