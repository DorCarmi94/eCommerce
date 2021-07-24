import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {Link, Redirect} from "react-router-dom";
import {UserApi} from "../Api/UserApi";
import {Item} from "../Data/Item";
import {StorePermission} from '../Data/StorePermission'
import {NavLink} from "reactstrap";

export default class Store extends Component {
    static displayName = Store.name;

    constructor(props) {
        super(props)
        const {storeId} = props
        this.state = {
            storeId: storeId,
            items: [],
            permissions:[],
            selectedItem:''
        }
        this.storeApi = new StoreApi();
    }

    async getItems(){
        const fetchedItems = await this.storeApi.getAllItems(this.state.storeId)
        if (fetchedItems && fetchedItems.isSuccess) {
            this.setState({
                items: fetchedItems.value
            })
        }
    }
    
    async componentDidMount() {
        await this.getItems();
        console.log(this.state.storeId)
        const fetchedPermissions = await this.storeApi.getStorePermissionForUser(this.state.storeId)
        console.log(fetchedPermissions)
        if(fetchedPermissions.isSuccess){
            console.log(fetchedPermissions.value)
            this.setState({
                permissions:fetchedPermissions.value
            })
        }
    }

    async componentDidUpdate(prevProps, prevState, undefined) {
        if (prevProps.storeId !== this.props.storeId) {
            console.log(`update `);
            console.log(this.props);
            console.log(prevProps);
            await this.setState({
                storeId: this.props.storeId
            })
            await this.getItems();
        }
    }
    
    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) {
            alert('succed')
            history.push(path);
        }
    }
    
    removeItem = async (storeId,itemId) =>
    {
        const res = await this.storeApi.removeItem(storeId,itemId)

        if(res && res.isSuccess) {
            alert('edit item succeed')
            // this.props.addStoreToState(storeId);
            this.redirectToHome('/')
        }
        else{
            if(res) {
                alert(`edit item failed because- ${res.error}`)
            }
        }

    }   
    
    storeManagment(){
        const {items,storeId,permissions} = this.state
        return (
            <div>
                {permissions.includes(StorePermission.ControlStaffPermission) ? <div><Link tag={Link} exact to={`/managePermissions/${storeId}`}>Manage Permissions</Link></div> : null}
                {permissions.includes(StorePermission.AddItemToStore) ? <div><Link to={`${storeId}/addItem`}>Add an Item</Link></div> : null}
                {permissions.includes(StorePermission.GetStoreHistory) ?<div> <Link to={`/purchaseHistory/${storeId}`}>Show Store's Purchase History</Link></div> : null}
                {permissions.includes(StorePermission.EditStorePolicy) ? <div><Link to={`/addRule/${storeId}`}>Add Rule To Policy</Link></div> : null}
                {permissions.includes(StorePermission.EditStorePolicy) ? <div><Link to={`/addPolicy/${storeId}`}>Add Discount To Policy</Link></div> : null}
                {permissions.includes(StorePermission.EditItemDetails) ? <div><Link to={`/showBids/${storeId}`}>Show Bids
                </Link></div> : null}


            </div>

        )
    }

    addToCart(itemName){
        this.setState({
            selectedItem:itemName
        })
    }
    
    render() {
        const {items,storeId,permissions,selectedItem} = this.state
        if(selectedItem.length > 0){
            return <Redirect exact to={`/searchItems/Item/${selectedItem}`}/>
        }
        if (items.length > 0) {
            return (
                <div>
                    {this.storeManagment()}
                    <Table striped bordered hover>
                    <thead>
                    <tr>
                        <th>#</th>
                        <th>Item name</th>
                        <th>Category</th>
                        <th>Amount</th>
                        <th>Price</th>
                    </tr>
                    </thead>
                    <tbody>

                    {
                        this.state.items.map((item, index) => {
                            return (
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{item.itemName}</td>
                                    <td>{item.category}</td>
                                    <td>{item.amount}</td>
                                    <td>{item.pricePerUnit}</td>
                                    <td>
                                        <div>
                                            {permissions.includes(StorePermission.EditItemDetails) ? <Link exact to={`${storeId}/editItem/${item.itemName}`}>Edit Item</Link> : null}
                                        </div>
                                        <div>
                                        {permissions.includes(StorePermission.RemoveStoreStaff) ? <button onClick={() => this.removeItem(storeId,item.itemName)}>Remove Item</button> : null }
                                        </div>
                                        <button onClick={(e) => this.addToCart(item.itemName) }>Add To Cart</button>
                                        <div><Link to = {`/bidItem/${item.storeName}/${item.itemName}`}>Bid Item</Link></div>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    </tbody>
                </Table>
                </div>
            );
        } else {
            return <div>
                {this.storeManagment()}
                Empty Store
            </div>
        }
    }
}
