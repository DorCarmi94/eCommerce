import React, { Component } from 'react';
import {BasketData} from "../../Data/CartData";
import {Item} from "../../Data/Item";
import "./Basket.css"
import {BasketItem} from "./BasketItem";

interface BasketProps {
    basket: BasketData,
    handleAmountUpdate: (storeId: string, itemId: string, amount: number) => void
}

export class Basket extends Component<BasketProps> {
    static displayName = Basket.name;

    constructor(props: BasketProps) {
        super(props);
    }

    renderItem(item: Item) {
        return (
            <BasketItem item={item} handleAmountUpdate={this.props.handleAmountUpdate}/>
        )
    }
    
    renderBasketItems(items: Item[]) {
        return items.map(item => this.renderItem(item))
    }

    render() {
        const basket = this.props.basket;
        return (
            <div>
                <h3>Store: {basket.storeId}</h3>
                {this.renderBasketItems(basket.items)}
            </div>
        );
    }
}