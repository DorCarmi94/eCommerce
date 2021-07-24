import React, { Component } from 'react';
import {CartApi} from "../Api/CartApi";
import {Basket} from "./Cart/Basket";
import {CartData} from "../Data/CartData";
import {Redirect} from "react-router-dom";

interface CartState {
    errorMessage: string | undefined,
    cart: CartData | undefined,
    totalPrice: number,
    redirectToPurchase: boolean,
}

export class Cart extends Component<{}, CartState> {
    static displayName = Cart.name;
    private cartApi: CartApi;

    constructor(props: any) {
        super(props);
        this.state = {
            errorMessage: undefined,
            cart: undefined,
            totalPrice: 0,
            redirectToPurchase: false
        };
        this.cartApi = new CartApi();
        
        this.handleAmountUpdate = this.handleAmountUpdate.bind(this);
        this.handleCartPurchase = this.handleCartPurchase.bind(this);
    }
    
    async handleAmountUpdate(storeId: string, itemId: string, amount: number){
        const updateRes = await this.cartApi.EditItemAmount(itemId, storeId, amount);
        if(!updateRes || updateRes.isFailure){
            this.setState({
                errorMessage: `Error in update: ${updateRes?.error}`,
                cart: undefined
            });
            return;
        }
        
        await this.updateTotalCartPrice()
    }
    
    async updateTotalCartPrice(){
        let cartTotalPriceRes = await this.cartApi.GetPurchasePrice();

        if(!cartTotalPriceRes || cartTotalPriceRes.isFailure){
            this.setState({
                errorMessage: `Error: ${cartTotalPriceRes?.error}`,
                cart: undefined,
                totalPrice: 0
            });
            return;
        }

        this.setState({
            totalPrice: cartTotalPriceRes.value
        })
    }
    
    async componentDidMount() {
        await this.getCart();
        await this.updateTotalCartPrice();
    }

    async getCart(){
        let cartRes = await this.cartApi.getCart();
        
        if(!cartRes || cartRes.isFailure){
            this.setState({
                errorMessage: `Error: ${cartRes?.error}`,
                cart: undefined,
                totalPrice: 0,
            });
            return;
        }

        this.setState({
            cart: cartRes.value,
        })
    }

    async handleCartPurchase() {
        this.setState({
            redirectToPurchase: true
        })
    }
    
    renderCart(cart: CartData){
        const renderedBaskets = cart.baskets.map(basket => <Basket basket={basket} handleAmountUpdate={this.handleAmountUpdate}/> );
        
        return (
            <div>
                {renderedBaskets.length == 0 ? <h4 className="None">Empty cart</h4> : 
                    renderedBaskets.concat(
                        <div style={{
                            display: "grid",
                            gridTemplateColumns: "1fr 1fr",
                            marginTop: "10px"
                        }}>
                            <h3>Total price: {this.state.totalPrice}</h3>
                            <button className="action" style={{margin: "auto"}}
                                    onClick={this.handleCartPurchase}>Purchse cart</button>
                        </div>
                    )}
            </div>
            
        ) 
    }
    
    render() {
        const { cart, errorMessage, redirectToPurchase } = this.state
        return (
            redirectToPurchase ? 
                <Redirect exact to="purchaseCart">Purchase cart</Redirect> :
                <main>
                { cart ?  this.renderCart(cart) : <h1>{errorMessage}</h1>}
                </main>
        );
    }
}