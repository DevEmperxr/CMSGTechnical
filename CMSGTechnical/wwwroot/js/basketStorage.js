window.basketStorage = {
    save: function (basket) {
        localStorage.setItem('basket', JSON.stringify(basket));
    },
    load: function () {
        const basket = localStorage.getItem('basket');
        return basket ? JSON.parse(basket) : null;
    },
    clear: function () {
        localStorage.removeItem('basket');
    }
};