window.productHub = {
    connection: null,
    start: function (dotNetRef) {
        if (this.connection) {
            return;
        }
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/products")
            .withAutomaticReconnect()
            .build();

        this.connection.on("ProductsChanged", function () {
            dotNetRef.invokeMethodAsync("OnProductsChanged");
        });

        this.connection.start().catch(function (error) {
            console.error("Product updates connection failed:", error);
        });
    },
    stop: function () {
        if (this.connection) {
            const connection = this.connection;
            this.connection = null;
            return connection.stop();
        }
    }
};
