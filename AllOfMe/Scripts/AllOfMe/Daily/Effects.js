/// <reference path="ViewModel.js" />
addEventListener("keydown", e => {
	if (e.keyCode === 27) {
		vm.CurrentPlan(null);
	}
});
function toggleForms(parent, show) {
	if (parent.dataset.last === show.id) {
		parent.dataset.last = "";
		parent.style.display = "none";
	} else {
		parent.dataset.last = show.id;
		parent.style.display = "";
		[...show.parentElement.children].forEach(e=>e.style.display = "none");
		show.style.display = "";
	}
}
