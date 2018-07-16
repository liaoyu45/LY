var i = 100, o = 200, a = 1, s = 20, c = 37, l = document.getElementById("bgAni"), r = l.getContext("2d"), d = [], u = {}, p = 0, h = 1.01, w = 2e-4, m = window.devicePixelRatio || 1, f = 0;
l.width = window.innerWidth * m, l.height = window.innerHeight * m, u.x = null, u.y = null, r.strokeStyle = "#8738f6", r.translate(.5, .5);


function n(e) {
	if (null !== u.x) {
		for (var n = 0; n < e; n++) {
			var t = { x: u.x, y: u.y, r: 1, speed: 1, accel: h, accel2: w, angle: f };
			f += c;
			var o = { x: i * Math.cos(t.angle * Math.PI / 180), y: i * Math.sin(t.angle * Math.PI / 180) };
			t.x += o.x, t.y += o.y, d.push(t)
		}
	}
}


function t() {
	n(s);
	var e = [];
	if (r.clearRect(0, 0, l.width, l.height), (p += a) < 360) {
		u = { x: l.width / 2, y: l.height / 2, angle: p };
		var t = u.angle * Math.PI / 180;
		u.x += o * Math.cos(t), u.y += o * Math.sin(t)
	} else {
		p = 0;
	}
	for (; d.length;) {
		var i = d.pop(),
			c = { x: i.speed * Math.cos(i.angle * Math.PI / 180), y: i.speed * Math.sin(i.angle * Math.PI / 180) };
		r.beginPath(),
		r.moveTo(i.x, i.y),
		r.lineTo(i.x + c.x, i.y + c.y),
		r.closePath(),
		r.stroke(),
		i.x += c.x, i.y += c.y, i.speed *= i.accel, i.accel += i.accel2, i.x < l.width && i.x > 0 && i.y < l.height && i.y > 0 && e.push(i)
	} d = e.slice(0).reverse()
}

function e() { requestAnimationFrame(e), t() }
e();