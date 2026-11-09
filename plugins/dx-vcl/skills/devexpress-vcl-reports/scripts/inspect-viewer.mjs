// usage: node inspect-viewer.mjs <outPrefix> [port]
const [,, out, port = '9222'] = process.argv;
import fs from 'node:fs';
const sleep = ms => new Promise(r => setTimeout(r, ms));
let targets;
for (let i = 0; i < 60; i++) {
  try { targets = await (await fetch(`http://127.0.0.1:${port}/json`)).json(); if (targets.some(t => t.type === 'page' && t.url.includes('/client/'))) break; } catch {}
  await sleep(1000);
}
fs.writeFileSync(out + '_targets.json', JSON.stringify(targets, null, 1));
const page = (targets || []).find(t => t.type === 'page' && t.url.includes('/client/'));
if (!page) { console.error(`no viewer page target (/client/) on port ${port}`); process.exit(1); }
const ws = new WebSocket(page.webSocketDebuggerUrl);
await new Promise(r => ws.onopen = r);
let id = 0; const pend = new Map();
ws.onmessage = m => { const d = JSON.parse(m.data); if (d.id && pend.has(d.id)) { pend.get(d.id)(d); pend.delete(d.id); } };
const send = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
const ev = async expr => (await send('Runtime.evaluate', { expression: expr, returnByValue: true, awaitPromise: true })).result?.result?.value;
await sleep(8000); // let viewer render
const info = await ev(`(() => ({
  url: location.href, lang: navigator.language, cookie: document.cookie,
  resources: performance.getEntriesByType('resource').map(e => e.name).filter(n => /locali|json/i.test(n)),
  text: document.body.innerText.slice(0, 3000),
  titles: [...document.querySelectorAll('[title]')].map(e => e.title).filter(Boolean).slice(0, 60)
}))()`);
fs.writeFileSync(out + '_info.json', JSON.stringify(info, null, 1));
const shot = await send('Page.captureScreenshot', { format: 'png' });
if (shot.result) fs.writeFileSync(out + '.png', Buffer.from(shot.result.data, 'base64'));
console.log(JSON.stringify(info, null, 1).slice(0, 4000));
ws.close();
