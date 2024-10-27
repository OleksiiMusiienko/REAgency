const items = document.querySelectorAll('.item');

items.forEach(item => {
    item.addEventListener('mouseenter', function () {
        const infoElement = item.querySelector('.info');


        infoElement.style.display = 'flex';
        setTimeout(() => {
            infoElement.classList.add('show');
        }, 30);
    });

    item.addEventListener('mouseleave', function () {
        const infoElement = item.querySelector('.info');


        if (infoElement.classList.contains('show')) {
            infoElement.classList.remove('show');
            setTimeout(() => {
                infoElement.style.display = 'none';
            }, 300);
        }
    });
});