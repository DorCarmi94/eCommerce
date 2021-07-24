import React, {ChangeEvent, Component} from "react";
import {Item} from "../Data/Item";
import {CartApi} from "../Api/CartApi";
import {Link,Redirect} from "react-router-dom";

import "./ItemDisplay.css"

interface ItemDisplayProps {
    item: Item
}

interface ItemDisplayState {
    quantity: number,
    errorMessage: string | undefined,
    successMessage: string | undefined
}

export class ItemDisplay extends Component<ItemDisplayProps, ItemDisplayState> {
    static displayName = ItemDisplay.name;
    private cartApi: CartApi;

    constructor(props: ItemDisplayProps) {
        super(props);

        this.state = {
            quantity: 0,
            errorMessage: undefined,
            successMessage: undefined
        }
        this.cartApi = new CartApi();
        
        this.handleQuantity = this.handleQuantity.bind(this);
        this.handleAddToCart = this.handleAddToCart.bind(this);

    }

    handleQuantity(add: number){
        const {item} = this.props;
        this.setState({
            quantity: this.state.quantity + add > item.amount ? item.amount :
                      this.state.quantity + add < 0 ? 0 : 
                          this.state.quantity + add
        })
    }

    async handleAddToCart(){
        const {item} = this.props;
        const addToCartRes = await this.cartApi.AddItem(item.itemName,
            item.storeName, this.state.quantity);
        if(addToCartRes){
            if(addToCartRes.isSuccess) {
                this.setState({
                    quantity: 0,
                    errorMessage: undefined,
                    successMessage: "Item has been added"
                })
            } else {
                this.setState({
                    errorMessage: addToCartRes.error,
                    successMessage: undefined
                })
            }
        } else {
            this.setState({
                errorMessage: "Connection error",   
                successMessage: undefined
            })
        }
    }

    renderCartSection(){
        const { errorMessage, successMessage } = this.state;
        return (
            <div className="cartSection">
                {errorMessage ? <label className="label3">{errorMessage}</label> : 
                 successMessage ? <label className="label4">{successMessage}</label> :
                 null}
                <label>Quantity: {this.state.quantity}</label>
                <button onClick={() => this.handleQuantity(1)}>+1</button>
                <button onClick={() => this.handleQuantity(-1)}>-1</button>
                <button className="action" onClick={this.handleAddToCart}>Add to cart</button>
            </div>
        )
    }

    render() {
        const {item} = this.props;
        return (
            <div className="itemDisplay">
                <label className="label1">{item.itemName}</label>
                <label className="label2">{item.storeName}</label>
                <label>Price per unit: {item.pricePerUnit}</label>
                <label>Catagory: {item.category}</label>
                {this.renderCartSection()}
                <Link to = {`/bidItem/${item.storeName}/${item.itemName}`}>Bid Item</Link>

            </div>
        )
    }
}