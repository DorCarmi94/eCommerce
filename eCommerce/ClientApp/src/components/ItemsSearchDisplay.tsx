import React, {Component} from "react";
import {Item} from "../Data/Item";
import "./ItemDisplay.css"
import {ItemDisplay} from "./ItemDisplay";
import {StoreApi} from "../Api/StoreApi";

interface ItemsSearchDisplayProps {
    itemQuery: string,
}

interface ItemsSearchDisplayState {
    items: Item[] | undefined
}

export class ItemSearchDisplay extends Component<ItemsSearchDisplayProps, ItemsSearchDisplayState> {
    static displayName = ItemSearchDisplay.name;
    private storeApi: StoreApi;

    constructor(props: ItemsSearchDisplayProps) {
        super(props);
        this.state = {
            items: undefined,
            
        }
        this.storeApi = new StoreApi();
    }

    async componentDidMount() {
        let res=undefined
            res = await this.storeApi.searchItems(this.props.itemQuery);
        
            if (res && res.isSuccess) {
                this.setState({
                    items: res.value
                })
            }
    }

    render() {
        const { items } = this.state;
        
        return (
            <div className="itemsDisplay">
                {items ?
                    items.map((item) => <ItemDisplay item={item}/>) :
                    "Getting items"}
            </div>
        )
    }
}