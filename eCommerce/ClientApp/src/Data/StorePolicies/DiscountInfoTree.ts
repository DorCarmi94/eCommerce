import {RuleNode} from "./RuleInfo";
import {Combinations} from "./Combinations";

export enum DiscountNodeType {
    Leaf,
    Composite,
}

export type DiscountNode = DiscountNodeLeaf | DiscountNodeComposite

export type DiscountNodeLeaf = {
    type: DiscountNodeType,
    rule: RuleNode,
    discount: number
}

export function makeDiscountNodeLeaf(rule: RuleNode, discount: number): DiscountNodeLeaf {
    return {
        type: DiscountNodeType.Leaf,
        rule: rule,
        discount: discount
    }
}

export type DiscountNodeComposite = {
    type: DiscountNodeType,
    discountA: DiscountNode,
    discountB: DiscountNode,
    combination: Combinations
}

export function makeDiscountCompositeNode(
    discountA: DiscountNode, discountB: DiscountNode, combination: Combinations): DiscountNodeComposite {
    return {
        type: DiscountNodeType.Composite,
        discountA: discountA,
        discountB: discountB,
        combination: combination
    }
}