export interface Summary {
    id?: number,
    name: string,
    semester: number,
    subject: string,
    keywords: string[],
    author?: string,
    isImageLoading?: boolean,
    image?: string
}