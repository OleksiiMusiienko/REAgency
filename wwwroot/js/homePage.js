$(document).ready(function () {
    const operationSelect = $('#operationSelect');
    const operationOptions = $('#operationOptions');
    const operationNativeSelect = $('#operationNativeSelect');

    const objectSelect = $('#objectSelect');
    const objectOptions = $('#objectOptions');
    const objectNativeSelect = $('#objectNativeSelect');

    const operationSelectedValue = operationNativeSelect.val();
    const operationSelectedText = operationNativeSelect.find(`option[value="${operationSelectedValue}"]`).text();

    const objectSelectedValue = objectNativeSelect.val();
    const objectSelectedText = objectNativeSelect.find(`option[value="${objectSelectedValue}"]`).text();

    operationNativeSelect.find('option').each(function () {
        const optionElement = $(this);
        const customOptionDiv = $('<div></div>')
            .addClass('custom-option')
            .text(optionElement.text())
            .attr('data-value', optionElement.val())
            .on('click', function () {
                operationSelect.find('span').text(optionElement.text());
                operationNativeSelect.val(optionElement.val());
                operationSelect.removeClass('active');
            });

        operationOptions.append(customOptionDiv);
    });

    objectNativeSelect.find('option').each(function () {
        const optionElement = $(this);
        const customOptionDiv = $('<div></div>')
            .addClass('custom-option')
            .text(optionElement.text())
            .attr('data-value', optionElement.val())
            .on('click', function () {
                objectSelect.find('span').text(optionElement.text());
                objectNativeSelect.val(optionElement.val());
                objectSelect.removeClass('active');
            });

        objectOptions.append(customOptionDiv);
    });

    
    if (operationSelectedText) {
        operationSelect.find('span').text(operationSelectedText);
    }

    if (objectSelectedText) {
        objectSelect.find('span').text(objectSelectedText);
    }

   
    operationSelect.on('click', function () {
        $(this).toggleClass('active');
    });
    objectSelect.on('click', function () {
        $(this).toggleClass('active');
    });

   
    $(document).on('click', function (event) {
        if (!operationSelect.is(event.target) && !operationSelect.has(event.target).length) {
            operationSelect.removeClass('active');
        }
    });
    $(document).on('click', function (event) {
        if (!objectSelect.is(event.target) && !objectSelect.has(event.target).length) {
            objectSelect.removeClass('active');
        }
    });
});

//фильтрация

document.getElementById('toggleButton').addEventListener('click', function () {
    const filtrationElement = document.getElementById('filtration');

    if (filtrationElement.classList.contains('show')) {
        filtrationElement.classList.remove('show');
        setTimeout(() => {
            filtrationElement.style.display = 'none';
        }, 300);
    } else {
        filtrationElement.style.display = 'block';
        setTimeout(() => {
            filtrationElement.classList.add('show');
        }, 10);
    }
});