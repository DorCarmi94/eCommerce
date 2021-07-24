import React, {Component} from "react";
import {Table,Button} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {Link,Redirect} from "react-router-dom";
import {UserApi} from "../Api/UserApi";
import {Item} from "../Data/Item";
import {StorePermission} from '../Data/StorePermission'

export default class ManagePermissions extends Component {
    static displayName = ManagePermissions.name;

    constructor(props) {
        super(props)
        const {storeId} = props
        this.state = {
            storeId: storeId,
            staff:[],
            changedManagers:[], // managers whose permissions have been changed
            submitted:false
        }
        this.storeApi = new StoreApi();
    }

    async getStaff(){
        const fetchedStaff = await this.storeApi.getStoreStaffPermissions(this.state.storeId)
        if (fetchedStaff && fetchedStaff.isSuccess) {
            this.setState({
                staff: fetchedStaff.value
            })
        }
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);

    }
    ADMIN_USERID ='_Admin'

    async componentDidMount() {
        const fetchedStaff = await this.getStaff();
        console.log(fetchedStaff)
        if(fetchedStaff && fetchedStaff.isSuccess){
            this.setState({
                staff:fetchedStaff.value
            })
        }
        // const fetchedPermissions = await this.storeApi.getStorePermissionForUser(this.state.storeId)
        // console.log(fetchedPermissions)
        // if(fetchedPermissions.isSuccess){
        //     console.log(fetchedPermissions.value)
        //     this.setState({
        //         permissions:fetchedPermissions.value
        //     })
        // }
    }

    // async componentDidUpdate(prevProps, prevState, undefined) {
    //     if (prevProps.storeId !== this.props.storeId) {
    //         console.log(`update `);
    //         console.log(this.props);
    //         console.log(prevProps);
    //         await this.setState({
    //             storeId: this.props.storeId
    //         })
    //         await this.getItems();
    //     }
    // }

    handleInputChange(event) {
        // const value =  target.checked : target.value;
        // const name = target.name;
        const detail = event.target.name.split('/')
        let editedStaff = this.state.staff
        if(!event.target.checked) {
            console.log(editedStaff[parseInt(detail[0])].permissions)
            editedStaff[parseInt(detail[0])].permissions = editedStaff[parseInt(detail[0])].permissions.filter((permission) => permission !== parseInt(detail[1]))
        }
        else{
            editedStaff[parseInt(detail[0])].permissions.push(parseInt(detail[1]))
        }
        console.log(editedStaff)
        // alert(detail)
        this.setState({
            staff : editedStaff,
            changedManagers : [... this.state.changedManagers,detail[2]]
        })
        
        
        // newStaff.detail
        //
        //
        // this.setState({
        //     staff:
        // });
    }
    
    
    handleSubmit(){
        const {staff,changedManagers,storeId} = this.state
        const updatePermissions = async (storeId,userId,permissions) =>
        {
           const res= await this.storeApi.updateManagerPermissions(storeId,userId,permissions)
           return res.isSuccess 
        }
        staff.map(async (member) => {
            if (changedManagers.includes(member.userId)) {
                if (! await updatePermissions(storeId, member.userId, member.permissions)) {
                    alert('Edit Permission Failed Has Not Fully Completed')
                }
                alert(`permissions for user:${member.userId} updated`)
            }
        })
        this.setState({
            submitted: true
        })
    }
    render() {
        // return <div>Permissions</div>
        // const {items,storeId,permissions} = this.state
        // if (items.length > 0) {
        const {staff, submitted,storeId} = this.state
        console.log(staff)
        if (submitted) {
            return <Redirect exact to={`/store/${storeId}`}/>
        } else {
            return (
                <div>
                    <h3>{`Permissions For The Store : ${this.state.storeId}`}</h3>
                    <div><Link to={`${this.state.storeId}/appointManager`}>Appoint Manager</Link></div>
                    {/*<div><Link to={`/removeManager/${this.state.storeId}`}>Remove Manager</Link></div>*/}
                    <div><Link to={`${this.state.storeId}/appointOwner`}>Appoint Owner</Link></div>
                    <div><Link to={`/removeOwner/${this.state.storeId}`}>Remove Owner</Link></div>


                    <Table striped bordered hover>
                        <thead>
                        <tr>
                            <th>User Id</th>
                            <th>Change Item Strategy</th>
                            <th>Add Item To Store</th>
                            <th>Change Item Price</th>
                            <th>Edit Item Details</th>
                            <th>Edit Store Policy</th>
                            <th>Get Store History</th>
                            <th>Control Staff Permission</th>
                            <th>Remove Store Staff</th>
                        </tr>
                        </thead>
                        <tbody>

                        {
                            staff.map((member, index) => {
                                const isAdmin = member.userId === this.ADMIN_USERID
                                return (
                                    <tr>
                                        <td>
                                            {member.userId}
                                        </td>
                                        <td>
                                            <input
                                                name={`${index}/0/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.ChangeItemStrategy)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                name={`${index}/1/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.AddItemToStore)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                name={`${index}/2/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.ChangeItemPrice)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                name={`${index}/3/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.EditItemDetails)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                name={`${index}/4/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.GetStoreHistory)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                        <input
                                            name={`${index}/5/${member.userId}`}
                                            type="checkbox"
                                            disabled={isAdmin}
                                            checked={member.permissions.includes(StorePermission.EditStorePolicy)}
                                            onChange={(e) => this.handleInputChange(e)}
                                        />
                                        <td>
                                            <input
                                                name={`${index}/6/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.ControlStaffPermission)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                name={`${index}/7/${member.userId}`}
                                                type="checkbox"
                                                disabled={isAdmin}
                                                checked={member.permissions.includes(StorePermission.RemoveStoreStaff)}
                                                onChange={(e) => this.handleInputChange(e)}
                                            />
                                        </td>
                                    </tr>
                                )
                            })
                        }
                        </tbody>
                    </Table>
                    <button className="action" onClick={this.handleSubmit}>Submit</button>
                </div>
            );

        }
    }
}
