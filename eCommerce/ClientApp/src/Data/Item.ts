export interface ItemType {
    itemName: string,
    storeName: string,
    amount: number,
    category: string,
    keyWords: string[],
    pricePerUnit: number,
}

export class Item implements ItemType{
    public itemName: string;
    public storeName: string;
    public amount: number;
    public category: string;
    public keyWords: string[];
    public pricePerUnit: number;
    
    constructor(data: ItemType) {
        this.itemName = data.itemName;
        this.storeName = data.storeName;
        this.amount = data.amount;
        this.category = data.category;
        this.keyWords = data.keyWords;
        this.pricePerUnit = data.pricePerUnit;
    }
    
    static createItem(itemName: string, storeName: string, amount: number,
        category: string, keyWords: string[], pricePerUnit: number): Item {
        return new Item({
            itemName: itemName,
            storeName: storeName,
            amount: amount,
            category: category,
            keyWords: keyWords,
            pricePerUnit: pricePerUnit
        })
    }
}