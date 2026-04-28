/**
 * AFRAWY STORE — POS (Point of Sale) JavaScript
 * Real-time cart management, product search, and checkout logic.
 */
(function () {
    'use strict';

    // ── State ──────────────────────────────────────────────
    let cart = [];           // Array of { productId, name, sku, unitPrice, costPrice, quantity, maxStock, unit }
    let allProducts = [];    // Cached product list from server
    let searchTimeout = null;
    let isCheckingOut = false;

    // ── DOM Refs ───────────────────────────────────────────
    const searchInput = document.getElementById('posSearch');
    const productsGrid = document.getElementById('productsGrid');
    const productsLoading = document.getElementById('productsLoading');
    const cartItemsContainer = document.getElementById('cartItems');
    const cartEmpty = document.getElementById('cartEmpty');
    const cartCountBadge = document.getElementById('cartCount');
    const subTotalEl = document.getElementById('subTotal');
    const grandTotalEl = document.getElementById('grandTotal');
    const discountInput = document.getElementById('discountInput');
    const btnCheckout = document.getElementById('btnCheckout');
    const btnClearCart = document.getElementById('btnClearCart');
    const checkoutError = document.getElementById('checkoutError');
    const checkoutErrorText = document.getElementById('checkoutErrorText');
    const antiForgeryToken = document.getElementById('antiForgeryToken')?.value;

    // ── Init ───────────────────────────────────────────────
    loadProducts('');

    searchInput.addEventListener('input', function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => loadProducts(this.value.trim()), 300);
    });

    discountInput.addEventListener('input', updateTotals);
    btnClearCart.addEventListener('click', clearCart);
    btnCheckout.addEventListener('click', checkout);

    // ── Product Search ─────────────────────────────────────
    function loadProducts(term) {
        productsLoading && (productsLoading.style.display = '');
        fetch(`/Sales/SearchProducts?term=${encodeURIComponent(term)}`)
            .then(res => res.json())
            .then(products => {
                allProducts = products;
                renderProducts(products);
            })
            .catch(() => {
                productsGrid.innerHTML = `
                    <div class="col-12 text-center py-5 text-danger">
                        <i class="bi bi-exclamation-triangle fs-3 d-block mb-2"></i>
                        حدث خطأ أثناء تحميل المنتجات
                    </div>`;
            });
    }

    function renderProducts(products) {
        if (productsLoading) productsLoading.style.display = 'none';

        if (products.length === 0) {
            productsGrid.innerHTML = `
                <div class="col-12 text-center py-5 text-muted">
                    <i class="bi bi-search fs-3 d-block mb-2 text-black-50"></i>
                    لا توجد منتجات مطابقة
                </div>`;
            return;
        }

        productsGrid.innerHTML = products.map(p => {
            const inCart = cart.find(c => c.productId === p.id);
            const effectiveStock = p.currentStock - (inCart ? inCart.quantity : 0);
            const isOutOfStock = effectiveStock <= 0;

            return `
                <div class="col-12 col-xl-6">
                    <div class="border rounded p-3 bg-white h-100 d-flex align-items-center gap-3 product-card ${isOutOfStock ? 'out-of-stock' : ''}"
                         ${!isOutOfStock ? `onclick="window._pos.addToCart(${p.id})"` : ''}>
                        <div class="bg-light rounded d-flex align-items-center justify-content-center text-muted flex-shrink-0" style="width: 64px; height: 64px;">
                            ${p.imagePath
                    ? `<img src="${p.imagePath}" alt="${p.name}" class="rounded" style="width:64px;height:64px;object-fit:cover;" />`
                    : `<i class="bi bi-image fs-4"></i>`}
                        </div>
                        <div class="flex-grow-1 overflow-hidden">
                            <div class="fw-bold fs-sm mb-1 text-truncate">${escapeHtml(p.name)}</div>
                            <div class="fs-caption text-muted mb-1">${escapeHtml(p.sku)} · <span class="fw-semibold text-dark numeric">${p.sellingPrice.toFixed(2)} ج</span></div>
                            <div class="fs-caption ${isOutOfStock ? 'text-danger fw-bold' : effectiveStock <= 5 ? 'text-warning' : 'text-success'}">
                                ${isOutOfStock
                    ? '<i class="bi bi-x-circle me-1"></i>نفذ المخزون'
                    : `المخزون: ${effectiveStock} ${escapeHtml(p.unit)}`}
                            </div>
                        </div>
                        ${!isOutOfStock ? `<button class="btn btn-sm btn-outline-primary rounded-pill px-3 flex-shrink-0" onclick="event.stopPropagation(); window._pos.addToCart(${p.id})">
                            <i class="bi bi-plus-lg"></i>
                        </button>` : ''}
                    </div>
                </div>`;
        }).join('');
    }

    // ── Cart Management ────────────────────────────────────
    function addToCart(productId) {
        const product = allProducts.find(p => p.id === productId);
        if (!product) return;

        const existing = cart.find(c => c.productId === productId);

        if (existing) {
            if (existing.quantity >= product.currentStock) {
                showError(`الكمية المطلوبة تتجاوز المخزون المتاح (${product.currentStock} ${product.unit})`);
                return;
            }
            existing.quantity++;
        } else {
            if (product.currentStock <= 0) {
                showError('هذا المنتج غير متوفر في المخزون');
                return;
            }
            cart.push({
                productId: product.id,
                name: product.name,
                sku: product.sku,
                unitPrice: product.sellingPrice,
                costPrice: product.costPrice,
                quantity: 1,
                maxStock: product.currentStock,
                unit: product.unit
            });
        }

        hideError();
        renderCart();
        updateTotals();
        renderProducts(allProducts); // Refresh stock display
    }

    function removeFromCart(productId) {
        cart = cart.filter(c => c.productId !== productId);
        hideError();
        renderCart();
        updateTotals();
        renderProducts(allProducts);
    }

    function updateQuantity(productId, delta) {
        const item = cart.find(c => c.productId === productId);
        if (!item) return;

        const newQty = item.quantity + delta;

        if (newQty <= 0) {
            removeFromCart(productId);
            return;
        }

        if (newQty > item.maxStock) {
            showError(`الكمية المطلوبة تتجاوز المخزون المتاح (${item.maxStock} ${item.unit})`);
            return;
        }

        item.quantity = newQty;
        hideError();
        renderCart();
        updateTotals();
        renderProducts(allProducts);
    }

    function clearCart() {
        if (cart.length === 0) return;
        cart = [];
        hideError();
        renderCart();
        updateTotals();
        renderProducts(allProducts);
    }

    function renderCart() {
        cartCountBadge.textContent = cart.length;

        if (cart.length === 0) {
            cartEmpty.style.display = '';
            // Clear any rendered cart items but keep the empty state
            const cartItems = cartItemsContainer.querySelectorAll('.cart-item');
            cartItems.forEach(el => el.remove());
            btnCheckout.disabled = true;
            return;
        }

        cartEmpty.style.display = 'none';
        btnCheckout.disabled = false;

        // Build cart HTML
        const cartHtml = cart.map(item => {
            const lineTotal = item.unitPrice * item.quantity;
            return `
                <div class="d-flex justify-content-between align-items-start mb-3 pb-3 border-bottom cart-item" data-product-id="${item.productId}">
                    <div class="flex-grow-1 pe-3">
                        <div class="fw-bold fs-sm mb-1">${escapeHtml(item.name)}</div>
                        <div class="text-muted fs-caption">${item.unitPrice.toFixed(2)} ج لكل ${escapeHtml(item.unit)}</div>
                        
                        <div class="d-flex align-items-center mt-2 gap-2">
                            <button class="qty-btn" onclick="window._pos.updateQuantity(${item.productId}, -1)">
                                <i class="bi bi-dash"></i>
                            </button>
                            <span class="fw-bold d-inline-block text-center numeric" style="width: 28px;">${item.quantity}</span>
                            <button class="qty-btn text-primary" onclick="window._pos.updateQuantity(${item.productId}, 1)">
                                <i class="bi bi-plus"></i>
                            </button>
                            <span class="fs-caption text-muted ms-1">(متاح: ${item.maxStock})</span>
                        </div>
                    </div>
                    <div class="text-end">
                        <div class="fw-bold numeric text-dark">${lineTotal.toFixed(2)} ج</div>
                        <button class="btn btn-link text-danger p-0 mt-2 fs-caption text-decoration-none" 
                                onclick="window._pos.removeFromCart(${item.productId})">
                            <i class="bi bi-x-lg me-1"></i>حذف
                        </button>
                    </div>
                </div>`;
        }).join('');

        // Replace all cart items (keep the empty-state div)
        const existingItems = cartItemsContainer.querySelectorAll('.cart-item');
        existingItems.forEach(el => el.remove());
        cartEmpty.insertAdjacentHTML('beforebegin', cartHtml);
    }

    // ── Totals ─────────────────────────────────────────────
    function updateTotals() {
        const subTotal = cart.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
        const discount = Math.max(0, parseFloat(discountInput.value) || 0);
        const grandTotal = Math.max(0, subTotal - discount);

        subTotalEl.textContent = subTotal.toFixed(2) + ' ج';
        grandTotalEl.textContent = grandTotal.toFixed(2) + ' ج';

        // Visual feedback if discount exceeds subtotal
        if (discount > subTotal && subTotal > 0) {
            discountInput.classList.add('is-invalid');
        } else {
            discountInput.classList.remove('is-invalid');
        }
    }

    // ── Checkout ───────────────────────────────────────────
    function checkout() {
        if (isCheckingOut || cart.length === 0) return;

        const subTotal = cart.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
        const discount = Math.max(0, parseFloat(discountInput.value) || 0);

        if (discount > subTotal) {
            showError('الخصم لا يمكن أن يكون أكبر من إجمالي المبلغ');
            return;
        }



        const note = document.getElementById('saleNote')?.value || null;

        const payload = {
            items: cart.map(c => ({ productId: c.productId, quantity: c.quantity })),
            discount: discount,

            note: note
        };

        isCheckingOut = true;
        btnCheckout.disabled = true;
        btnCheckout.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>جاري المعالجة...';
        hideError();

        fetch('/Sales/Checkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            },
            body: JSON.stringify(payload)
        })
            .then(res => res.json())
            .then(result => {
                if (result.success) {
                    // Redirect to receipt
                    window.location.href = `/Sales/Detail/${result.saleId}`;
                } else {
                    showError(result.error || 'حدث خطأ غير متوقع');
                    resetCheckoutButton();
                }
            })
            .catch(err => {
                showError('حدث خطأ في الاتصال. حاول مرة أخرى.');
                resetCheckoutButton();
            });
    }

    function resetCheckoutButton() {
        isCheckingOut = false;
        btnCheckout.disabled = cart.length === 0;
        btnCheckout.innerHTML = '<i class="bi bi-check2-circle"></i>إتمام البيع';
    }

    // ── Error Handling ─────────────────────────────────────
    function showError(message) {
        checkoutErrorText.textContent = message;
        checkoutError.classList.remove('d-none');
        // Auto-hide after 5 seconds
        setTimeout(hideError, 5000);
    }

    function hideError() {
        checkoutError.classList.add('d-none');
    }

    // ── Utility ────────────────────────────────────────────
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // ── Expose to global (for inline onclick handlers) ────
    window._pos = {
        addToCart,
        removeFromCart,
        updateQuantity
    };

})();
