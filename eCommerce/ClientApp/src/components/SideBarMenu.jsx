import React, {Component} from "react";
import {Menu, MenuItem, ProSidebar, SidebarContent, SidebarHeader, SubMenu} from "react-pro-sidebar";
import "./SideBarMenu.css"
import {Link} from "react-router-dom";
import {NavItem, NavLink} from "reactstrap";
import {UserRole} from "../Data/UserRole";

export class SideBarMenu extends Component {
    static displayName = SideBarMenu.name;

    constructor(props) {
        super(props);
        this.state = {
            memberIconJsx: undefined,
            adminIconJsx: undefined
        }
    }
    
    renderOwnedStoreListMenu() {
        const { ownedStoreList } = this.props;
        if(!ownedStoreList || ownedStoreList.length === 0) {
            return null;
        }
        
        return (
            <SubMenu title="Owned Stores">
                {ownedStoreList.map ((store) =>
                    (
                        <MenuItem>
                            <NavLink tag={Link} exact to={`/store/${store}`}>{store}</NavLink>
                        </MenuItem>
                        
                    )
                )}
            </SubMenu>
        )
    }
    
    componentDidMount() {
        this.setState({
            memberIconJsx: <img className="imgSideBar" src="Images/member.png" alt={null}/>,
            adminIconJsx: <img className="imgSideBar" src="Images/admin.png" alt={null}/>
        })
    }


    renderManagedStoreListMenu() {
        const { managedStoreList } = this.props;
        if(!managedStoreList ||managedStoreList.length === 0) {
            return null;
        }

        return (
            <SubMenu title="Managed Stores">
                {managedStoreList.map ((store) =>
                    (
                        <MenuItem>
                            <NavLink tag={Link} exact to={`/store/${store}`}>{store}</NavLink>
                        </MenuItem>
                    )
                )}
            </SubMenu>
        )
    }
    
    render() {
        const { role } = this.props
        const { memberIconJsx, adminIconJsx } = this.state;
        console.log(role)
        return (
            <ProSidebar breakPoint="md" className="sideBarContainer">
                <SidebarHeader>
                    <div className="sideBarHeaderContainer">
                        Managment Panel
                    </div>
                </SidebarHeader>
    
                <SidebarContent>
                    <Menu className="menuLayout">
                        <SubMenu title="Member panel"
                                 // <img class="imgSideBar" src="Images/member.png" alt={null}/>
                            icon={memberIconJsx}>
                        <MenuItem>
                            <NavLink tag={Link} exact to="/openStore">Create new store</NavLink>
                        </MenuItem>
                        <MenuItem>
                            <NavLink tag={Link} exact to="/purchaseHistory">My Purchase History</NavLink>
                        </MenuItem>
                            {this.renderOwnedStoreListMenu()}
                            {this.renderManagedStoreListMenu()}
                        </SubMenu>
                    </Menu>

                    {role === UserRole.Admin ?
                        <Menu className="menuLayout">
                            <SubMenu title="Admin panel"
                                 //<img className="imgSideBar" src="Images/admin.png" alt={null}/>
                                     icon={adminIconJsx}>
                                <MenuItem>
                                    <NavLink tag={Link} exact to={`/AdminPurchaseHistory/Store`}>Store Purchase History</NavLink>
                                </MenuItem>
                                <MenuItem>
                                    <NavLink tag={Link} exact to={`/AdminPurchaseHistory/User`}>User Purchase History</NavLink>
                                </MenuItem>
                                <MenuItem>
                                    <NavLink tag={Link} exact to={`/showStats`}>Show System Stats</NavLink>
                                </MenuItem>
                            </SubMenu>
                        </Menu> :
                        null}
                </SidebarContent>
        </ProSidebar>)
    }
}