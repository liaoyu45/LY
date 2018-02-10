function myfunction(width, height, fx, fy, dx, dy) {
	var k = getK(dx, dy, fx, fy);
	var kk = getK(0, 0, fx, fx);
	var kkk = getK(width, 0, fx, fy);
	var kkkk = getK(width, height, fx, fy);
	var kkkkk = getK(0, height, fx, fy);
	if ((k > kk || k < kkk) && dy < fy) {
		return Math.abs(dy / fy);
	}
	if (k > kkk && k < kkkk && dx > fx) {
		return Math.abs(width - dx) / (width / fx);
	}
	if ((k > kkkk || k < kkkkk) && dy > fy) {
		return Math.abs(height - dy) / (height - fy);
	}
	return Math.abs(dx / dy);
}
function getK(x, y, xx, yy) {
	return (yy - y) / (xx - x);
}