
(function () {
    if (!String.prototype.format) {
        Object.defineProperty(String.prototype, 'format', {
            value: function () {
                var s = this,
                    i = arguments.length;

                while (i--) {
                    s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
                }
                return s;
            },
            enumerable: false
        });
    }

} ());

(function (g, $, ko) {
    var ProductFactory, productVm, groceryVm, addProductVm,
        updateGrocereyList, updateProductList, updateGroceryItem, removeGroceryItem,
        removeProduct, clearGroceries, addProductToList, displayErrorDialog,
        getData, addToProducts, removeFromProducts, sortProducts,
        initialize;

    // Models
    ProductFactory = (function () {
        var ProductFactory = function (id, name) {
            this.id = ko.observable(id);
            this.name = ko.observable(name);
            this.removeGrocery = function () {
                removeGroceryItem(this.id());
            };
            this.removeProduct = function () {
                removeProduct(this.id());
            };
            this.addToGrocery = function () {
                updateGroceryItem(this.id());
            };
            this.toggleCheck = function () {
                $(arguments[1].currentTarget).find('img.on').toggle();
                $(arguments[1].currentTarget).find('img.off').toggle();
            };
        };

        return function (id, name) {
            return new ProductFactory(id, name);
        };
    })();

    groceryVm = {
        groceryArray: ko.observableArray(),
        setEmptyMessage: function () {
            if (this.groceryArray().length === 0) {
                return "No items are on the lists";
            }

            return "";
        },
        isDirty: false,
        showList: function () {
            return this.groceryArray().length > 0;
        }
    };

    productVm = {
        productArray: ko.observableArray(),
        isDirty: false,
        showList: function () {
            return this.productArray().length > 0;
        }
    };

    addProductVm = {
        name: ko.observable(""),
        addToList: ko.observable(false),
        message: ko.observable(""),

        addProduct: function () {
            if (this.name().length > 0) {
                addProductToList(this.name(), this.addToList());
            } else {
                this.message("No product entered!");
            }
        },

        reset: function () {
            this.name("");
            this.message("");
            this.addToList(false);
            $("#addToList").checkboxradio("refresh");
        },

        resetInputs: function () {
            this.name("");
        }
    };

    initialize = {
        initListPage: function () {
            var defaults = {}, $page;

            $.extend(defaults, arguments[0]);

            $page = $("#MPGroceries");

            $page.bind("pageshow", function () {
                if (groceryVm.isDirty) {
                    $page.find("#listGrocery").listview("refresh");
                    groceryVm.isDirty = false;
                }
            });

            $page.find('#clear').click(function () {
                clearGroceries();
            });
        },

        initProductPage: function () {
            var defaults = {}, $page;

            $.extend(defaults, arguments[0]);

            $page = $("#MPProducts");

            $page.bind("pageshow", function () {
                if (productVm.isDirty) {
                    $page.find("#listProduct").listview("refresh");
                    productVm.isDirty = false;
                }
            });
        },

        initAddProductPage: function () {
            var defaults = {}, $page;

            $.extend(defaults, arguments[0]);

            $page = $("#MPAddProduct");

            $page.bind("pageshow", function () {
                addProductVm.reset();
            });
        },

        initDialogPage: function () {
            var $page = $("#MPDialog");

            $page.bind("pageshow", function () {
                // Do somthing
            });
        },

        initErrorDialogPage: function () {
            var $page = $("#MPError");

            $page.bind("pageshow", function () {
                // Do somthing
            });
        },
        
        initData: function () {
            ko.applyBindings(groceryVm, $("#MPGroceries").get(0));
            ko.applyBindings(productVm, $("#MPProducts").get(0));
            ko.applyBindings(addProductVm, $("#MPAddProduct").get(0));

            $.when(updateGrocereyList(), updateProductList()).always(function () {
                
                $("#Loading").hide();
                $("#AfterLoading").removeClass("h");
                $("section").removeClass("h");
                
                $.mobile.hidePageLoadingMsg();
            });
        },

        initAllPages: function () {
            $.mobile.showPageLoadingMsg();
            g.initApp("initListPage");
            g.initApp("initProductPage");
            g.initApp("initAddProductPage");
            g.initApp("initDialogPage");
            g.initApp("initErrorDialogPage");
            g.initApp("initData");
        }
    };

    g.initApp = function (page) {
        if (initialize[page]) {
            return initialize[page].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof page === 'object' || !page) {
            return initialize.initAllPages.apply(this, arguments);
        } else {
            $.error('Method ' + page + ' does not exist');
            return undefined;
        }
    };

    updateGrocereyList = function () {

        var $page = $("#MPGroceries"),
            url = "/home/groceries/",
            success,
            failure;

        success = function (data) {
            var i, $list;

            $list = $page.find("#listGrocery");

            if (data.length === 0) {
                return;
            }

            for (i = 0; i < data.length; i++) {
                groceryVm.groceryArray.push(new ProductFactory(data[i].ProductId, data[i].ProductName));
            }

            if ($.mobile.activePage && $.mobile.activePage.attr('id') === $page.attr('id')) {
                $list.listview("refresh");
            }

        };

        failure = function () {
            displayErrorDialog();
        };

        return getData(url, 'GET', success, failure, null);

    };

    updateProductList = function () {

        var $page = $("#MPProducts"),
            url = "/home/products/",
            success,
            failure;

        success = function (data) {
            var $list, i;
            $list = $page.find("#listProduct");

            if (data.length === 0) {
                return;
            }

            for (i = 0; i < data.length; i++) {
                productVm.productArray.push(new ProductFactory(data[i].Id, data[i].Name));
            }

            if ($.mobile.activePage && $.mobile.activePage.attr('id') === $page.attr('id')) {
                $list.listview("refresh");
            }

        };

        failure = function () {
            displayErrorDialog();
        };

        return getData(url, 'GET', success, failure, null);

    };

    updateGroceryItem = function (productId) {

        var url = "/home/addgrocery/{0}".format(productId),
            success,
            failure;

        success = function () {
            var product = productVm.productArray.remove(function (item) {
                return item.id() === parseInt(productId, 10);
            });

            groceryVm.groceryArray.push(product[0]);

            removeFromProducts(product[0]);

            groceryVm.isDirty = true;
            $("#MPProducts").find("#listProduct").listview("refresh");
        };

        failure = function () {
            displayErrorDialog();
        };

        getData(url, 'GET', success, failure, null);
    };

    removeGroceryItem = function (productId) {

        var url = "/home/removegrocery/{0}".format(productId),
            success,
            failure;

        success = function () {
            var grocery = groceryVm.groceryArray.remove(function (item) {
                return item.id() === parseInt(productId, 10);
            });

            addToProducts(grocery[0]);
            sortProducts();

            productVm.isDirty = true;
            $("#MPGroceries").find("#listGrocery").listview("refresh");
        };

        failure = function () {
            displayErrorDialog();
        };

        getData(url, 'GET', success, failure, null);
    };

    removeProduct = function (id) {
        var $page, $ok;

        $page = $('#MPDialog');
        $ok = $page.find('a#ok');

        $ok.unbind('click');
        $ok.bind('click', function () {

            var url = "/home/removeproduct/{0}".format(id),
                success,
                failure;

            success = function () {
                var product = productVm.productArray.remove(function (item) {
                    return item.id() === parseInt(id, 10);
                });

                removeFromProducts(product[0]);

                $("#MPProducts").find("#listProduct").listview();

                $page.dialog('close');
            };

            failure = function () {
                displayErrorDialog();
            };

            getData(url, 'GET', success, failure, null);

        });

        $('#show-delete-page').click();

    };

    clearGroceries = function () {
        var $page, $ok;

        $page = $('#MPDialog');
        $ok = $page.find('a#ok');

        $ok.unbind('click');
        $ok.bind("click", function () {

            var url = "/home/cleargroceries/",
                success,
                failure;

            success = function () {
                var $groceryPage = $("#MPGroceries");

                ko.utils.arrayForEach(groceryVm.groceryArray(), function (item) {
                    addToProducts(item);
                });

                sortProducts();

                groceryVm.groceryArray.removeAll();

                $groceryPage.find("#listGrocery").listview("refresh");
                productVm.isDirty = true;

                $page.dialog('close');
            };

            failure = function () {
                displayErrorDialog();
            };

            getData(url, 'GET', success, failure, null);

        });

        $('#show-delete-page').click();
    };

    addProductToList = function (name, addToList) {

        var url = "/home/addproduct",
            success,
            failure;

        success = function (data) {
            var newProduct;

            if (!data) {
                displayErrorDialog();
                return;
            }

            if (data.Id > 0) {
                newProduct = new ProductFactory(data.Id, data.Name);

                if (addToList) {
                    groceryVm.groceryArray.push(newProduct);
                    groceryVm.isDirty = true;
                } else {
                    addToProducts(newProduct);
                    sortProducts();
                    productVm.isDirty = true;
                }
            }

            addProductVm.message(data.Message);
            addProductVm.resetInputs();
        };

        failure = function () {
            displayErrorDialog();
        };

        getData(url, 'POST', success, failure, { product: name, addToList: addToList });

    };

    displayErrorDialog = function () {
        $("#show-error-page").click();
    };

    getData = function (url, action, success, failure, postData) {
        $.mobile.showPageLoadingMsg();

        return $.ajax({
            type: action,
            url: url,
            dataType: 'json',
            data: postData,
            success: function (data) {
                success(data);
                $.mobile.hidePageLoadingMsg();
            },
            error: function () {
                failure();
                $.mobile.hidePageLoadingMsg();
            }
        });
    };

    addToProducts = function (product) {
        var startingLetter = product.name().substr(0, 1).toLowerCase();

        var exists = ko.utils.arrayFirst(productVm.productArray(), function (item) {
            return item.name().toLowerCase() === startingLetter;
        });

        if (!exists) {
            productVm.productArray.push(new ProductFactory(-1, startingLetter));
        }

        productVm.productArray.push(product);
    };

    removeFromProducts = function (product) {
        var count, startingLetter;

        count = 0;
        startingLetter = product.name().substr(0, 1).toLowerCase();

        ko.utils.arrayForEach(productVm.productArray(), function (item) {
            if (item.name().substr(0, 1).toLowerCase() === startingLetter) {
                count++;
            }
        });

        if (count === 1) {
            productVm.productArray.remove(function (item) {
                return item.name() === startingLetter;
            });
        }
    };

    sortProducts = function () {
        productVm.productArray.sort(function (item1, item2) {
            return item2.name().toLowerCase() > item1.name().toLowerCase() ? -1 : item2.name().toLowerCase() === item1.name().toLowerCase() ? 0 : 1;
        });
    };

} (window.grocery = window.grocery || {}, jQuery, ko));
