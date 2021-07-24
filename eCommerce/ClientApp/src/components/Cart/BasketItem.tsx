import React, {ChangeEvent, Component} from "react";
import {Item} from "../../Data/Item";
import "./Basket.css"

interface BasketItemProps {
    item: Item,
    handleAmountUpdate: (storeId: string, itemId: string, amount: number) => void
}

interface BasketItemState {
    amount: number
}

export class BasketItem extends Component<BasketItemProps, BasketItemState> {
    static displayName = BasketItem.name;
    
    constructor(props: BasketItemProps) {
        super(props);
        this.state = {
            amount: props.item.amount
        }
        
        this.handleAmountUpdate = this.handleAmountUpdate.bind(this);
        this.handleUpdate = this.handleUpdate.bind(this);

    }

    handleAmountUpdate(event: ChangeEvent<HTMLInputElement>){
        const parsed = parseInt(event.target.value);
        if(isNaN(parsed) || parsed < 0){
            alert("invalid quantity");
            return;
        }
        
        this.setState({
            amount: parsed
        });
    }

    handleUpdate(){
        const { item } = this.props;
        this.props.handleAmountUpdate(item.storeName, item.itemName, this.state.amount);
    }

    render() {
        const {item} = this.props;
        return (
            <div className="horizontalBorders">
                <div className="basketItemsContainer">
                    <label>Item: {item.itemName}</label>
                    <div className="inputAmountContainer">
                        Quantity: <input value={this.state.amount} onChange={this.handleAmountUpdate}/>
                        <button className="action" onClick={this.handleUpdate}>Update</button>
                    </div>
                </div>
            </div>
        )
    }
}